// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Collections.Immutable;

namespace Beryl.RHI.Resources;

/// <summary>
/// Describes a parameter exposed by a shader.
/// </summary>
public sealed class ShaderParameter
{
	/// <summary> The type of the parameter. </summary>
	public enum ParamType
	{
		Int,
		Float,
		Matrix,
		Vector,
		SampledTexture2D
	}

	public string Name { get; }
	public ParamType Type { get; }
	public object? DefaultValue { get; }

	public ShaderParameter(string name, ParamType type, object? defaultValue = null)
	{
		Name = name;
		Type = type;
		DefaultValue = defaultValue;
	}
}

/// <summary>
/// Describes a shader resource binding.
/// </summary>
public sealed class ShaderResource
{
	public string Name { get; }
	public ResourceKind Kind { get; }
	public uint Binding { get; }
	public uint Set { get; }
	public uint SizeInBytes { get; }

	public ImmutableArray<ShaderParameter> Parameters { get; }

	public ShaderResource(string name, ResourceKind kind, uint binding, uint set, IEnumerable<ShaderParameter> parameters, uint sizeInBytes)
	{
		Name = name;
		Kind = kind;
		Binding = binding;
		Set = set;
		SizeInBytes = sizeInBytes;
		Parameters = parameters.ToImmutableArray();
	}
}

/// <summary>
/// A group of shader resources that share a resource set.
/// </summary>
public readonly struct ShaderResourceGroup
{
	public uint Set { get; init; }
	public ImmutableArray<ShaderResource> Resources { get; init; }
	public ImmutableArray<ResourceLayoutElementDescription> LayoutElements { get; init; }
}
