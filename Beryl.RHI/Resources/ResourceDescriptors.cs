// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Standard;
using System.Runtime.CompilerServices;

namespace Beryl.RHI.Resources;

public readonly ref struct NamedBufferDescriptor(string name, uint size, BufferUsage usage) : IAlwaysHashable
{
	public readonly uint Size = size;
	public readonly BufferUsage Usage = usage;
	public readonly ReadOnlySpan<char> Name = name;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		int hash = unchecked(string.GetHashCode(Name));

		hash = hash * 31 + (int)Size;
		hash = hash * 31 + (int)Usage;

		return hash;
	}
}

public readonly ref struct BufferDescriptor(uint size, BufferUsage usage) : IAlwaysHashable
{
	public readonly uint Size = size;
	public readonly BufferUsage Usage = usage;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(Size, Usage);
}



public readonly ref struct PipelineDescriptor(string passName, ShaderDescriptor vertShader, ShaderDescriptor fragShader, IFramebuffer output, ReadOnlySpan<ShaderResourceGroup> resourceGroups, CullingMode cullingMode = CullingMode.Back) : IAlwaysHashable
{
	public readonly string PassName = passName;
	public readonly ShaderDescriptor VertShader = vertShader;
	public readonly ShaderDescriptor FragShader = fragShader;
	public readonly IFramebuffer Output = output;
	public readonly ReadOnlySpan<ShaderResourceGroup> ResourceGroups = resourceGroups;
	public readonly CullingMode CullingMode = cullingMode;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hash = new HashCode();
		hash.Add(PassName);
		hash.Add(VertShader.GetHashCode());
		hash.Add(FragShader.GetHashCode());
		hash.Add(Output);
		hash.Add(CullingMode);
		foreach (ref readonly var group in ResourceGroups)
		{
			hash.Add(group.Set);
			foreach (var element in group.LayoutElements)
			{
				hash.Add(element.Name);
				hash.Add(element.Kind);
				hash.Add(element.Stages);
			}
		}
		return hash.ToHashCode();
	}
}


public readonly ref struct SamplerDescriptor(Sampler sampler) : IAlwaysHashable
{
	public readonly Sampler Sampler = sampler;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 31 + (int)Sampler.Filter;
			hash = hash * 31 + (int)Sampler.WrapU;
			hash = hash * 31 + (int)Sampler.WrapV;
			return hash;
		}
	}
}

public readonly ref struct TextureDescriptor(uint width, uint height, TextureFormat format, ReadOnlySpan<byte> data) : IAlwaysHashable
{
	public readonly uint Width = width;
	public readonly uint Height = height;
	public readonly TextureFormat Format = format;
	public readonly ReadOnlySpan<byte> Data = data;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 31 + (int)Width;
			hash = hash * 31 + (int)Height;
			hash = hash * 31 + (int)Format;
			foreach (byte b in Data)
				hash = hash * 31 + b;
			return hash;
		}
	}
}

public readonly ref struct TextureViewDescriptor(ITexture texture) : IAlwaysHashable
{
	public readonly ITexture Texture = texture;

	/// <inheritdoc/>
	public override int GetHashCode() => RuntimeHelpers.GetHashCode(Texture);
}

public readonly struct ResourceLayoutElementDescription(string name, ResourceKind kind, ShaderStages stages)
{
	public readonly string Name = name;
	public readonly ResourceKind Kind = kind;
	public readonly ShaderStages Stages = stages;
}

public readonly ref struct ResourceLayoutDescriptor(ReadOnlySpan<ResourceLayoutElementDescription> elements) : IAlwaysHashable
{
	public readonly ReadOnlySpan<ResourceLayoutElementDescription> Elements = elements;

	/// <inheritdoc/>
	public int Hash => GetHashCode();

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hash = new HashCode();
		foreach (ref readonly var element in Elements)
		{
			hash.Add(element.Name);
			hash.Add(element.Kind);
			hash.Add(element.Stages);
		}
		return hash.ToHashCode();
	}
}

public readonly ref struct ResourceDescriptor(IResourceLayout layout, ReadOnlySpan<IBindableResource> boundResources) : IAlwaysHashable
{
	public readonly IResourceLayout Layout = layout;
	public readonly ReadOnlySpan<IBindableResource> BoundResources = boundResources;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var hash = new HashCode();
		hash.Add(Layout);
		foreach (IBindableResource resource in BoundResources)
			hash.Add(resource);
		return hash.ToHashCode();
	}
}

public readonly ref struct ShaderDescriptor(ReadOnlySpan<byte> bytecode, ShaderStages stage) : IAlwaysHashable
{
	public readonly ReadOnlySpan<byte> Bytecode = bytecode;
	public readonly ShaderStages Stage = stage;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;

			foreach (byte b in Bytecode)
				hash = hash * 31 + b;

			return hash;
		}
	}
}
