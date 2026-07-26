// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;

namespace Beryl.RHI;

/// <summary>
/// Defines how an <see cref="IGraphicsDevice"/> creates GPU resources.
/// </summary>
public interface IResourceFactory
{
	/// <summary> Creates a new <see cref="ICommandBuffer"/>. </summary>
	ICommandBuffer CreateCommandBuffer();

	/// <summary> Creates a new <see cref="IShader"/> using the given <see cref="ShaderDescriptor"/> information. </summary>
	IShader CreateShader(ShaderDescriptor key);

	/// <summary> Creates a new <see cref="IBuffer"/> using the given <see cref="BufferDescriptor"/> information. </summary>
	IBuffer CreateBuffer(BufferDescriptor key);

	/// <summary> Creates a new <see cref="ITexture"/> using the given <see cref="TextureDescriptor"/> information. </summary>
	ITexture CreateTexture(TextureDescriptor key);

	/// <summary> Creates a new <see cref="ITextureView"/> using the given <see cref="TextureViewDescriptor"/> information. </summary>
	ITextureView CreateTextureView(TextureViewDescriptor key);

	/// <summary> Creates a new <see cref="ISampler"/> using the given <see cref="SamplerDescriptor"/> information. </summary>
	ISampler CreateSampler(SamplerDescriptor key);

	/// <summary> Creates a new <see cref="IResourceLayout"/> using the given <see cref="ResourceLayoutDescriptor"/> information. </summary>
	IResourceLayout CreateResourceLayout(ResourceLayoutDescriptor key);

	/// <summary> Creates a new <see cref="IResourceSet"/> using the given <see cref="ResourceDescriptor"/> information. </summary>
	IResourceSet CreateResourceSet(ResourceDescriptor key);

	/// <summary> Creates a new <see cref="IPipeline"/> using the given <see cref="PipelineDescriptor"/> information. </summary>
	IPipeline CreateGraphicsPipeline(PipelineDescriptor key);
}
