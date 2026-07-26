// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;
using Veldrith;

namespace Beryl.RHI.VeldrithBackend;

internal sealed class VeldrithResourceFactory(VeldrithDevice device) : IResourceFactory
{
	/// <inheritdoc/>
	public ICommandBuffer CreateCommandBuffer() => new VeldrithCommandBuffer(device);

	/// <inheritdoc/>
	public IShader CreateShader(ShaderDescriptor key)
	{
		Veldrith.ShaderStages stage = key.Stage == Resources.ShaderStages.Vertex
			? Veldrith.ShaderStages.Vertex
			: Veldrith.ShaderStages.Fragment;

		var description = new ShaderDescription(stage, key.Bytecode.ToArray(), "main");
		return new VeldrithResources(device.GraphicsDevice.ResourceFactory.CreateShader(description));
	}

	/// <inheritdoc/>
	public IBuffer CreateBuffer(BufferDescriptor key)
	{
		var description = new BufferDescription(key.Size, ToVeldrith(key.Usage));
		return new VeldrithBuffer(device.GraphicsDevice.ResourceFactory.CreateBuffer(description));
	}

	/// <inheritdoc/>
	public ITexture CreateTexture(TextureDescriptor key)
	{
		var description = new TextureDescription(
			key.Width,
			key.Height,
			1,
			1,
			1,
			PixelFormat.R8G8B8A8UNorm,
			TextureUsage.Sampled,
			TextureType.Texture2D);
		Texture texture = device.GraphicsDevice.ResourceFactory.CreateTexture(description);

		device.GraphicsDevice.UpdateTexture(texture, key.Data, 0, 0, 0, key.Width, key.Height, 1, 0, 0);
		return new VeldrithTexture(texture);
	}

	/// <inheritdoc/>
	public ITextureView CreateTextureView(TextureViewDescriptor key)
	{
		TextureView textureView = device.GraphicsDevice.ResourceFactory.CreateTextureView(((VeldrithTexture)key.Texture).Resource);
		return new VeldrithTextureView(textureView);
	}

	/// <inheritdoc/>
	public ISampler CreateSampler(SamplerDescriptor key)
	{
		var description = new SamplerDescription
		{
			Filter = key.Sampler.Filter == Resources.SamplerFilter.Nearest
				? Veldrith.SamplerFilter.MinPointMagPointMipPoint
				: Veldrith.SamplerFilter.MinLinearMagLinearMipPoint,
			AddressModeU = key.Sampler.WrapU == TextureWrapMode.Clamp
				? Veldrith.SamplerAddressMode.Clamp
				: Veldrith.SamplerAddressMode.Wrap,
			AddressModeV = key.Sampler.WrapV == TextureWrapMode.Clamp
				? Veldrith.SamplerAddressMode.Clamp
				: Veldrith.SamplerAddressMode.Wrap
		};

		return new VeldrithSampler(device.GraphicsDevice.ResourceFactory.CreateSampler(description));
	}

	/// <inheritdoc/>
	public IResourceLayout CreateResourceLayout(ResourceLayoutDescriptor key)
	{
		Veldrith.ResourceLayoutElementDescription[] elements = key.Elements.ToArray()
			.Select(element => new Veldrith.ResourceLayoutElementDescription(
				element.Name,
				Veldrith.ResourceKind.UniformBuffer,
				ToVeldrith(element.Stages)))
			.ToArray();
		var description = new ResourceLayoutDescription(elements);

		return new VeldrithResourceLayout(device.GraphicsDevice.ResourceFactory.CreateResourceLayout(description));
	}

	/// <inheritdoc/>
	public IResourceSet CreateResourceSet(ResourceDescriptor key)
	{
		var description = new ResourceSetDescription(
			((VeldrithResourceLayout)key.Layout).Resource,
			key.BoundResources.ToArray().Select(ToVeldrith).ToArray());

		return new VeldrithResourceSet(device.GraphicsDevice.ResourceFactory.CreateResourceSet(description));
	}

	/// <inheritdoc/>
	public IPipeline CreateGraphicsPipeline(PipelineDescriptor key)
	{
		ResourceLayout[] layouts = key.ResourceGroups.ToArray()
			.Select(group => ((VeldrithResourceLayout)CreateResourceLayout(new ResourceLayoutDescriptor(group.LayoutElements.AsSpan()))).Resource)
			.ToArray();
		var vertexLayout = new VertexLayoutDescription(
			new VertexElementDescription("POSITION", VertexElementSemantic.Position, VertexElementFormat.Float3),
			new VertexElementDescription("NORMAL", VertexElementSemantic.Normal, VertexElementFormat.Float3),
			new VertexElementDescription("TEXCOORD0", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));
		var shaderSet = new ShaderSetDescription(
			[vertexLayout],
			[((VeldrithResources)CreateShader(key.VertShader)).Resource, ((VeldrithResources)CreateShader(key.FragShader)).Resource]);
		var description = new GraphicsPipelineDescription
		{
			BlendState = BlendStateDescription.SINGLE_OVERRIDE_BLEND,
			DepthStencilState = new DepthStencilStateDescription(true, true, ComparisonKind.LessEqual),
			RasterizerState = ToVeldrith(key.CullingMode),
			PrimitiveTopology = PrimitiveTopology.TriangleList,
			ShaderSet = shaderSet,
			ResourceLayouts = layouts,
			Outputs = ((VeldrithFramebuffer)key.Output).Framebuffer.OutputDescription
		};

		return new VeldrithPipeline(device.GraphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref description));
	}

	private static Veldrith.BufferUsage ToVeldrith(Resources.BufferUsage usage)
	{
		Veldrith.BufferUsage result = 0;

		if (usage.HasFlag(Resources.BufferUsage.VertexBuffer))
			result |= Veldrith.BufferUsage.VertexBuffer;
		if (usage.HasFlag(Resources.BufferUsage.IndexBuffer))
			result |= Veldrith.BufferUsage.IndexBuffer;
		if (usage.HasFlag(Resources.BufferUsage.UniformBuffer))
			result |= Veldrith.BufferUsage.UniformBuffer;
		if (usage.HasFlag(Resources.BufferUsage.Dynamic))
			result |= Veldrith.BufferUsage.Dynamic;

		return result;
	}

	private static Veldrith.ShaderStages ToVeldrith(Resources.ShaderStages stages)
	{
		Veldrith.ShaderStages result = 0;

		if (stages.HasFlag(Resources.ShaderStages.Vertex))
			result |= Veldrith.ShaderStages.Vertex;
		if (stages.HasFlag(Resources.ShaderStages.Fragment))
			result |= Veldrith.ShaderStages.Fragment;

		return result;
	}

	private static RasterizerStateDescription ToVeldrith(CullingMode culling) => culling switch
	{
		CullingMode.Back => new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, true),
		CullingMode.Front => new RasterizerStateDescription(FaceCullMode.Front, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, true),
		_ => RasterizerStateDescription.CULL_NONE
	};

	private static Veldrith.IBindableResource ToVeldrith(Resources.IBindableResource resource) => resource switch
	{
		VeldrithBuffer buffer => buffer.Resource,
		VeldrithTexture texture => texture.Resource,
		VeldrithTextureView view => view.Resource,
		VeldrithSampler sampler => sampler.Resource,
		_ => throw new NotSupportedException($"Unsupported Veldrith resource type '{resource.GetType().Name}'.")
	};
}
