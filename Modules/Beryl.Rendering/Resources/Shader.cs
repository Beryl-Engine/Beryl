// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Resources.DefaultProvider;
using Beryl.Common.Utility;
using Beryl.RHI;
using Beryl.RHI.Resources;
using SlangShaderSharp;
using System.Collections.Immutable;
using System.Text;

namespace Beryl.Rendering.Resources;

public class Shader
{
	/// <summary> The <see cref="IShaderDefaultsProvider"/> used by shaders. </summary>
	/// <remarks> Defaults to <see cref="Rendering.ShaderDefaultsProvider"/>. </remarks>
	public static IShaderDefaultsProvider DefaultsProvider { get; } = new ShaderDefaultsProvider();

	/// <summary> The name of the shader's pass. </summary>
	public string Pass { get; internal set; } = "None";

	/// <summary> The name of the shader. This is used to import this shader. </summary>
	public string Name { get; internal set; }

	/// <summary> The shader's raw Slang source. </summary>
	/// <remarks> Setting this value will recompile the shader and is a heavy operation. </remarks>
	public string Source
	{
		get => field;
		set
		{
			SlangCompilationResult comp = ShaderBaker.SlangToSpirV(value, Name);
			VertexBytecode = comp.VertexBytes.ToArray();
			FragmentBytecode = comp.FragmentBytes.ToArray();

			foreach (var attribute in comp.ShaderAttributes)
			{
				if (attribute.Name == "ShaderPass")
					Pass = attribute.GetArgumentValueString(0);

				if (attribute.Name == "CullMode")
				{
					if (Enum.TryParse(attribute.GetArgumentValueString(0), true, out CullingMode mode))
						CullingMode = mode;
					else
						BerylConsole.Warning($"Unsupported culling mode '{attribute.GetArgumentValueString(0)}'.");
				}
			}

			var resources = new List<ShaderResource>();

			foreach (var resource in comp.Resources)
			{
				var parameters = new List<ShaderParameter>();

				for (uint i = 0; i < resource.TypeLayout.ElementTypeLayout.FieldCount; i++)
				{
					var @field = resource.TypeLayout.ElementTypeLayout.GetFieldByIndex(i);

					if (!Enum.TryParse(@field.Type.Name, true, out ShaderParameter.ParamType type))
					{
						BerylConsole.Warning($"Unsupported shader parameter type '{@field.Type.Name}'.");
						continue;
					}

					ShaderValue? defaultValue = null;

					for (uint j = 0; j < @field.Variable.AttributeCount; j++)
					{
						var attribute = @field.Variable.GetAttribute(j);

						// Hacky
						switch (attribute.Name)
						{
							case "DefaultFloat":
								defaultValue = new ShaderValue.Float(attribute.GetArgumentValueFloat(0));
								break;

							case "DefaultInt":
								defaultValue = new ShaderValue.Int(attribute.GetArgumentValueInt(0));
								break;

							case "DefaultFloat2":
								defaultValue = new ShaderValue.Vector(attribute.GetArgumentValueFloat(0), attribute.GetArgumentValueFloat(1));
								break;

							case "DefaultFloat3":
								defaultValue = new ShaderValue.Vector(attribute.GetArgumentValueFloat(0), attribute.GetArgumentValueFloat(1), attribute.GetArgumentValueFloat(2));
								break;

							case "DefaultFloat4":
								defaultValue = new ShaderValue.Vector(attribute.GetArgumentValueFloat(0), attribute.GetArgumentValueFloat(1), attribute.GetArgumentValueFloat(2), attribute.GetArgumentValueFloat(3));
								break;

							case "DefaultTexture":
								defaultValue = new ShaderValue.SampledTexture2D();
								break;
						}
					}

					parameters.Add(new ShaderParameter(@field.Name, type, defaultValue));
				}

				var elementType = resource.TypeLayout;

				uint size = (uint)elementType.ElementTypeLayout.GetSize(SlangParameterCategory.Uniform);

				resources.Add(new ShaderResource(
					resource.Name,
					ResourceKind.UniformBuffer,
					(uint)resource.GetOffset(SlangParameterCategory.DescriptorTableSlot),
					(uint)resource.GetOffset(SlangParameterCategory.SubElementRegisterSpace),
					parameters,
					size));
			}

			Resources = resources.OrderBy(r => r.Set).ThenBy(r => r.Binding).ToImmutableArray();

			var layouts = Resources.Select(r => new ResourceLayoutElementDescription(r.Name, r.Kind, ShaderStages.Vertex | ShaderStages.Fragment)).ToImmutableArray();

			ResourceGroups = Resources
				.GroupBy(r => r.Set)
				.Select(g => new ShaderResourceGroup
				{
					Set = g.Key,
					Resources = g.OrderBy(r => r.Binding).ToImmutableArray(),
					LayoutElements = layouts
				})
				.ToImmutableArray();

			field = value;
		}
	}

	/// <summary> The shader's resource definitions. </summary>
	public ImmutableArray<ShaderResource> Resources { get; private set; } = ImmutableArray<ShaderResource>.Empty;

	/// <summary> The shader's resource definitions, grouped by set. </summary>
	public ImmutableArray<ShaderResourceGroup> ResourceGroups { get; private set; }

	/// <summary> The shader's raw SPIR-V bytecode. </summary>
	internal byte[] VertexBytecode { get; private set; } = Array.Empty<byte>();

	/// <summary> The shader's raw SPIR-V bytecode. </summary>
	internal byte[] FragmentBytecode { get; private set; } = Array.Empty<byte>();

	/// <summary> The shader's requested <see cref="RHI.CullingMode"/>. </summary>
	internal CullingMode CullingMode { get; private set; } = CullingMode.Back;

	/// <summary> Initializes a new instance of the <see cref="Shader"/> class. </summary>
	/// <param name="source">The slang source code.</param>
	/// <param name="name">The name of the shader. This is used to import this shader into other shaders.</param>
	public Shader(string source, string name = "None")
	{
		Name = name;
		Source = source;
	}
}

[ResourceLoader]
internal sealed class ShaderLoader : ResourceLoader<Shader>
{
	/// <inheritdoc/>
	public override Shader? LoadResource(byte[] data, string? name = null) => new(Encoding.UTF8.GetString(data), name ?? "None");
}
