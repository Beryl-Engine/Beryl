// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;
using WebGpuSharp;

namespace Beryl.RHI.WebGPUBackend;

// WebGPUSharp owns native references through managed handles. Clear them on disposal;
// buffers and textures additionally destroy their GPU storage explicitly.

internal sealed class WebGPUFramebuffer(WebGPUDevice device) : IFramebuffer
{
	internal TextureView ColorView => device.GetCurrentView();
	internal TextureView DepthView => device.DepthView;
	internal WebGpuSharp.TextureFormat Format => device.SurfaceFormat;

	/// <inheritdoc/>
	public void Dispose() => device.ReleaseCurrentTexture();
}

internal sealed class WebGPUShader(ShaderModule resource) : IShader
{
	internal ShaderModule Resource { get; private set; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource = null!;
}

internal sealed class WebGPUBuffer(WebGpuSharp.Buffer resource, uint size) : IBuffer
{
	internal WebGpuSharp.Buffer Resource { get; private set; } = resource;

	/// <inheritdoc/>
	public uint SizeInBytes { get; } = size;

	/// <inheritdoc/>
	public void Dispose()
	{
		Resource?.Destroy();
		Resource = null!;
	}
}

internal sealed class WebGPUTexture(Texture resource) : ITexture
{
	internal Texture Resource { get; private set; } = resource;

	/// <inheritdoc/>
	public void Dispose()
	{
		Resource?.Destroy();
		Resource = null!;
	}
}

internal sealed class WebGPUTextureView(TextureView resource) : ITextureView
{
	internal TextureView Resource { get; private set; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource = null!;
}

internal sealed class WebGPUSampler(WebGpuSharp.Sampler resource) : ISampler
{
	internal WebGpuSharp.Sampler Resource { get; private set; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource = null!;
}

internal sealed class WebGPUResourceLayout(BindGroupLayout resource, int count) : IResourceLayout
{
	internal BindGroupLayout Resource { get; private set; } = resource;
	internal int Count { get; } = count;

	/// <inheritdoc/>
	public void Dispose() => Resource = null!;
}

internal sealed class WebGPUResourceSet(BindGroup resource) : IResourceSet
{
	internal BindGroup Resource { get; private set; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource = null!;
}

internal sealed class WebGPUPipeline(RenderPipeline resource) : IPipeline
{
	internal RenderPipeline Resource { get; private set; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource = null!;
}

internal sealed class WebGPUComputePipeline(ComputePipeline resource) : IComputePipeline
{
	internal ComputePipeline Resource { get; private set; } = resource;

	/// <inheritdoc/>
	public void Dispose() => Resource = null!;
}
