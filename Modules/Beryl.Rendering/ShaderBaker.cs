// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
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
			switch (backend)
			{
				case RHI.RendererBackend.Vulkan:
					return SlangCompileTarget.Spirv;
				case RHI.RendererBackend.Direct3D12:
					return SlangCompileTarget.Dxil;
				default:
					throw new NotImplementedException($"Can't compile Slang for target {backend}.");
			}
		}
	}

	/// <summary> The <see cref="SlangProfileID"/> for the current rendering backend. </summary>
	public static SlangProfileID SlangProfile
	{
		get
		{
			switch (backend)
			{
				case RHI.RendererBackend.Vulkan:
					return globalSession.FindProfile("spirv_1_3");
				case RHI.RendererBackend.Direct3D12:
					return globalSession.FindProfile("sm_6_0");
				default:
					throw new NotImplementedException($"Can't compile Slang for target {backend}.");
			}
		}
	}

	private static RHI.RendererBackend backend => ModuleManager.GetModule<RenderingModule>()?.Backend ?? RHI.RendererBackend.Vulkan;

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

		ReadOnlySpan<AttributeReflection> attributes = GetAttributes(module);
		ReadOnlySpan<VariableReflection> parameters = GetParameters(module);
		List<VariableLayoutReflection> resources = new();

		List<(IEntryPoint EntryPoint, SlangStage Stage)> entryPoints = GetEntryPoints(module).ToList();

		if (entryPoints.Count == 0)
			return new SlangCompilationResult { ShaderAttributes = attributes, ShaderParameters = parameters, Resources = resources.ToArray() };

		List<IComponentType> components = [module];
		components.AddRange(entryPoints.Select(e => (IComponentType)e.EntryPoint));

		localSession.CreateCompositeComponentType(components.ToArray(), out IComponentType program, out _);
		program.Link(out IComponentType linkedProgram, out _);

		var linkedLayout = linkedProgram.GetLayout(0, out _);

		for (uint i = 0; i < linkedLayout.ParameterCount; i++)
		{
			VariableLayoutReflection variable = linkedLayout.GetParameterByIndex(i);

			if (variable.Type.Kind == SlangTypeKind.ParameterBlock || variable.Type.Kind == SlangTypeKind.ConstantBuffer)
				resources.Add(variable);
		}

		Dictionary<SlangStage, byte[]> stages = new();

		for (int i = 0; i < entryPoints.Count; i++)
		{
			linkedProgram.GetEntryPointCode(i, 0, out ISlangBlob blob, out _);
			int size = (int)blob.GetBufferSize();

			byte[] bytes = new byte[size];
			unsafe
			{
				new ReadOnlySpan<byte>(blob.GetBufferPointer(), size).CopyTo(bytes);
			}

			stages[entryPoints[i].Stage] = bytes;
		}

		return new SlangCompilationResult
		{
			Stages = stages,
			ShaderAttributes = attributes,
			ShaderParameters = parameters,
			Resources = resources.ToArray()
		};
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

	private static ReadOnlySpan<AttributeReflection> GetAttributes(IModule module)
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

		return attributes.ToArray();
	}

	private static ReadOnlySpan<VariableReflection> GetParameters(IModule module)
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

		return parameters.ToArray();
	}
}

/// <summary>
/// Result of a Slang compilation.
/// </summary>
public readonly ref struct SlangCompilationResult
{
	/// <summary> All shader stages present in the compilation. </summary>
	public IReadOnlyDictionary<SlangStage, byte[]> Stages { get; init; }

	/// <summary> All attributes inside of a <c>SHADER_ATTRIBUTES()</c> block. </summary>
	/// <example>
	/// The following is Slang source that defines a <c>ShaderPass</c> Shader Attribute:
	/// <code>
	/// SHADER_ATTRIBUTES(
	///     [ShaderPass("Opaque")]
	/// )
	/// </code>
	/// </example>
	public ReadOnlySpan<AttributeReflection> ShaderAttributes { get; init; }

	/// <summary> All parameters inside of a <c>SHADER_PARAMETERS()</c> block. </summary>
	public ReadOnlySpan<VariableReflection> ShaderParameters { get; init; }

	/// <summary> All resources this shader requests. </summary>
	public ReadOnlySpan<VariableLayoutReflection> Resources { get; init; }
}
