// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using SlangShaderSharp;

namespace Beryl.Rendering;

/// <summary>
/// Handles baking Slang module source code into a <see cref="SlangCompilationResult"/>.
/// </summary>
public static class ShaderBaker
{
	/// <summary> The <see cref="SlangCompileTarget"/> for the current rendering backend. </summary>
	public static SlangCompileTarget SlangTarget
	{
		get
		{
			switch (RenderingModule.Backend)
			{
				case RHI.RendererBackend.Vulkan:
					return SlangCompileTarget.Spirv;
				case RHI.RendererBackend.Direct3D12:
					return SlangCompileTarget.Dxil;
				default:
					throw new NotImplementedException($"Can't compile Slang for target {RenderingModule.Backend}.");
			}
		}
	}

	/// <summary> The <see cref="SlangProfileID"/> for the current rendering backend. </summary>
	public static SlangProfileID SlangProfile
	{
		get
		{
			switch (RenderingModule.Backend)
			{
				case RHI.RendererBackend.Vulkan:
					return globalSession.FindProfile("spirv_1_3");
				case RHI.RendererBackend.Direct3D12:
					return globalSession.FindProfile("sm_6_0");
				default:
					throw new NotImplementedException($"Can't compile Slang for target {RenderingModule.Backend}.");
			}
		}
	}

	private static readonly IGlobalSession globalSession;
	private static readonly ISession localSession;

	static ShaderBaker()
	{
		Slang.CreateGlobalSession(Slang.ApiVersion, out var gs);
		globalSession = gs;

		CompilerOptionEntry[] options =
		[
			new CompilerOptionEntry
				{
					Name = CompilerOptionName.MatrixLayoutColumn,
					Value = new CompilerOptionValue
					{
						Kind = CompilerOptionValueKind.Int,
						IntValue0 = 1
					}
				}
		];


		SessionDesc sesDesc = new()
		{
			Targets = [new TargetDesc { Format = SlangTarget, Profile = SlangProfile }],
			SearchPaths = ["Assets"], // Hacky
			CompilerOptionEntries = options,
		};

		globalSession.CreateSession(sesDesc, out var ls);
		localSession = ls;
	}

	/// <summary> Compiles Slang source to SPIRV. </summary>
	/// <param name="source"> The string slang source code. </param>
	/// <param name="moduleName"> The name of the Slang module. This is used for importing this compilation into other modules. </param>
	public static SlangCompilationResult SlangToSpirV(string source, string moduleName)
	{
		IModule? module = localSession.LoadModuleFromSourceString(moduleName, $"{moduleName}.slang", source, out ISlangBlob? diagnostics);
		if (module == null)
			throw new Exception($"Failed to compile shader: {diagnostics?.AsString}");

		IEntryPoint? vertexEntry = null;
		IEntryPoint? fragmentEntry = null;

		var entryPoints = GetEntryPoints(module);
		vertexEntry = entryPoints.FirstOrDefault(x => x.Stage == SlangStage.Vertex).EntryPoint;
		fragmentEntry = entryPoints.FirstOrDefault(x => x.Stage == SlangStage.Fragment).EntryPoint;

		List<AttributeReflection> attributes = GetAttributes(module);
		List<VariableReflection> parameters = GetParameters(module);

		ReadOnlySpan<byte> vert = Span<byte>.Empty;
		ReadOnlySpan<byte> frag = Span<byte>.Empty;

		List<VariableLayoutReflection> resources = new();

		if (vertexEntry == null && fragmentEntry == null)
			return new SlangCompilationResult() { ShaderAttributes = attributes, ShaderParameters = parameters, Resources = resources };

		List<IComponentType> components = [module];

		if (vertexEntry != null)
			components.Add(vertexEntry);

		if (fragmentEntry != null)
			components.Add(fragmentEntry);

		localSession.CreateCompositeComponentType(components.ToArray(), out IComponentType program, out _);
		program.Link(out IComponentType linkedProgram, out _);

		var linkedLayout = linkedProgram.GetLayout(0, out _);

		for (uint i = 0; i < linkedLayout.ParameterCount; i++)
		{
			VariableLayoutReflection variable = linkedLayout.GetParameterByIndex(i);

			if (variable.Type.Kind == SlangTypeKind.ParameterBlock || variable.Type.Kind == SlangTypeKind.ConstantBuffer)
				resources.Add(variable);
		}

		if (vertexEntry != null)
		{
			linkedProgram.GetEntryPointCode(0, 0, out ISlangBlob vertBlob, out _);
			int vertBufferSize = (int)vertBlob.GetBufferSize();

			unsafe
			{ vert = new ReadOnlySpan<byte>(vertBlob.GetBufferPointer(), vertBufferSize); }
		}

		if (fragmentEntry != null)
		{
			linkedProgram.GetEntryPointCode(1, 0, out ISlangBlob fragBlob, out _);
			int fragBufferSize = (int)fragBlob.GetBufferSize();

			unsafe
			{ frag = new ReadOnlySpan<byte>(fragBlob.GetBufferPointer(), fragBufferSize); }
		}

		return new SlangCompilationResult() { FragmentBytes = frag, VertexBytes = vert, ShaderAttributes = attributes, ShaderParameters = parameters, Resources = resources };
	}

	private static IEnumerable<(IEntryPoint EntryPoint, SlangStage Stage)> GetEntryPoints(IModule module)
	{
		for (int i = 0; i < module.GetDefinedEntryPointCount(); i++)
		{
			module.GetDefinedEntryPoint(i, out var entryPoint);
			ShaderReflection reflection = entryPoint.GetLayout(0, out _);
			yield return (entryPoint, reflection.GetEntryPointByIndex(0).Stage);
		}
	}

	private static List<AttributeReflection> GetAttributes(IModule module)
	{
		List<AttributeReflection> attributes = new();

		ShaderReflection layout = module.GetLayout(0, out _);
		TypeReflection? attributesType = layout.FindTypeByName("ShaderAttributes");
		if (attributesType != null)
		{
			uint aCount = attributesType.Value.AttributeCount;

			for (uint i = 0; i < aCount; i++)
			{
				var attribute = attributesType.Value.GetAttribute(i);
				attributes.Add(attribute);
			}
		}

		return attributes;
	}

	private static List<VariableReflection> GetParameters(IModule module)
	{
		List<VariableReflection> parameters = new();

		ShaderReflection layout = module.GetLayout(0, out _);
		TypeReflection? parametersType = layout.FindTypeByName("ShaderParameters");
		if (parametersType != null)
		{
			uint pCount = parametersType.Value.FieldCount;

			for (uint i = 0; i < pCount; i++)
			{
				var parameter = parametersType.Value.GetFieldByIndex(i);
				parameters.Add(parameter);
			}
		}

		return parameters;
	}
}

/// <summary>
/// Result of a Slang compilation.
/// </summary>
public readonly ref struct SlangCompilationResult
{
	/// <summary> SPIRV Vertex bytes. </summary>
	public ReadOnlySpan<byte> VertexBytes { get; init; }

	/// <summary> SPIRV Fragment bytes. </summary>
	public ReadOnlySpan<byte> FragmentBytes { get; init; }

	/// <summary> All attributes inside of a <c>SHADER_ATTRIBUTES()</c> block. </summary>
	/// <example>
	/// The following is Slang source that defines a <c>ShaderPass</c> Shader Attribute:
	/// <code>
	/// SHADER_ATTRIBUTES(
	///     [ShaderPass("Opaque")]
	/// )
	/// </code>
	/// </example>
	public List<AttributeReflection> ShaderAttributes { get; init; }

	/// <summary> All parameters inside of a <c>SHADER_PARAMETERS()</c> block. </summary>
	public List<VariableReflection> ShaderParameters { get; init; }

	/// <summary> All resources this shader requests. </summary>
	public List<VariableLayoutReflection> Resources { get; init; }
}
