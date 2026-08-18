// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;

namespace Beryl.RHI.VulkanBackend;

internal sealed class VulkanResourceFactory(VulkanDevice device) : IResourceFactory
{
	/// <inheritdoc/>
	public IBuffer CreateBuffer(BufferDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public ICommandBuffer CreateCommandBuffer() => throw new NotImplementedException();

	/// <inheritdoc/>
	public IPipeline CreateGraphicsPipeline(PipelineDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public IResourceLayout CreateResourceLayout(ResourceLayoutDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public IResourceSet CreateResourceSet(ResourceDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public ISampler CreateSampler(SamplerDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public IShader CreateShader(ShaderDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public ITexture CreateTexture(TextureDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public ITextureView CreateTextureView(TextureViewDescriptor key) => throw new NotImplementedException();
}
