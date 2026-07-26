// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;
using Veldrith;

namespace Beryl.RHI.VeldrithBackend;

internal sealed class VeldrithFramebuffer(Framebuffer framebuffer) : IFramebuffer
{
	internal Framebuffer Framebuffer { get; } = framebuffer;

	/// <inheritdoc/>
	public void Dispose() => Framebuffer.Dispose();
}


internal sealed class VeldrithResources(Shader resource) : IShader
{
	internal Shader Resource { get; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource.Dispose();
}

internal sealed class VeldrithBuffer(DeviceBuffer resource) : IBuffer
{
	internal DeviceBuffer Resource { get; } = resource;

	/// <inheritdoc/>
	public uint SizeInBytes => Resource.SizeInBytes;

	/// <inheritdoc/>
	public void Dispose() => Resource.Dispose();
}

internal sealed class VeldrithTexture(Texture resource) : ITexture
{
	internal Texture Resource { get; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource.Dispose();
}

internal sealed class VeldrithTextureView(TextureView resource) : ITextureView
{
	internal TextureView Resource { get; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource.Dispose();
}

internal sealed class VeldrithSampler(global::Veldrith.Sampler resource) : ISampler
{
	internal global::Veldrith.Sampler Resource { get; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource.Dispose();
}

internal sealed class VeldrithResourceLayout(ResourceLayout resource) : IResourceLayout
{
	internal ResourceLayout Resource { get; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource.Dispose();
}

internal sealed class VeldrithResourceSet(ResourceSet resource) : IResourceSet
{
	internal ResourceSet Resource { get; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource.Dispose();
}

internal sealed class VeldrithPipeline(Pipeline resource) : IPipeline
{
	internal Pipeline Resource { get; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource.Dispose();
}
