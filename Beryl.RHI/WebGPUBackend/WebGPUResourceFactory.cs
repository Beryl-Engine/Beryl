// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;
using WebGpuSharp;

using BufferDescriptor = Beryl.RHI.Resources.BufferDescriptor;
using ComputePipelineDescriptor = Beryl.RHI.Resources.ComputePipelineDescriptor;
using SamplerDescriptor = Beryl.RHI.Resources.SamplerDescriptor;
using TextureDescriptor = Beryl.RHI.Resources.TextureDescriptor;
using TextureViewDescriptor = Beryl.RHI.Resources.TextureViewDescriptor;

namespace Beryl.RHI.WebGPUBackend;

internal sealed class WebGPUResourceFactory(WebGPUDevice device) : IResourceFactory
{
	/// <inheritdoc/>
	public IBuffer CreateBuffer(BufferDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public ICommandBuffer CreateCommandBuffer() => throw new NotImplementedException();

	/// <inheritdoc/>
	public IComputePipeline CreateComputePipeline(ComputePipelineDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public IPipeline CreateGraphicsPipeline(PipelineDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public IResourceLayout CreateResourceLayout(ResourceLayoutDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public IResourceSet CreateResourceSet(ResourceDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public ISampler CreateSampler(SamplerDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public IShader CreateShader(ShaderDescriptor key)
	{
		ShaderModuleWGSLDescriptor descriptor = new()
		{
			Code = key.Bytecode
		};

		ShaderModule module = device.Device.CreateShaderModuleWGSL(in descriptor);
		return new WebGPUShader(module);
	}

	/// <inheritdoc/>
	public ITexture CreateTexture(TextureDescriptor key) => throw new NotImplementedException();

	/// <inheritdoc/>
	public ITextureView CreateTextureView(TextureViewDescriptor key) => throw new NotImplementedException();
}
