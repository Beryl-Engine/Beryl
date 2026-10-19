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
	public ICommandBuffer CreateCommandBuffer() => new WebGPUCommandBuffer(device);

	/// <inheritdoc/>
	public IBuffer CreateBuffer(BufferDescriptor key)
	{
		WebGpuSharp.BufferUsage usage = WebGpuSharp.BufferUsage.CopyDst;
		if (key.Usage.HasFlag(Resources.BufferUsage.VertexBuffer))
			usage |= WebGpuSharp.BufferUsage.Vertex;
		if (key.Usage.HasFlag(Resources.BufferUsage.IndexBuffer))
			usage |= WebGpuSharp.BufferUsage.Index;
		if (key.Usage.HasFlag(Resources.BufferUsage.UniformBuffer))
			usage |= WebGpuSharp.BufferUsage.Uniform;
		ArgumentOutOfRangeException.ThrowIfZero(key.Size);
		return new WebGPUBuffer(device.Device.CreateBuffer(new WebGpuSharp.BufferDescriptor
		{
			Size = ((ulong)key.Size + 3) & ~3UL,
			Usage = usage
		}), key.Size);
	}

	/// <inheritdoc/>
	public IShader CreateShader(ShaderDescriptor key) => new WebGPUShader(device.Device.CreateShaderModuleWGSL(new ShaderModuleWGSLDescriptor { Code = key.Bytecode }));

	/// <inheritdoc/>
	public ITexture CreateTexture(TextureDescriptor key)
	{
		ArgumentOutOfRangeException.ThrowIfZero(key.Width);
		ArgumentOutOfRangeException.ThrowIfZero(key.Height);

		int pixels = checked((int)(key.Width * key.Height));
		int channels = key.Format switch
		{
			Resources.TextureFormat.RGBA8 => 4,
			Resources.TextureFormat.RGB8 => 3,
			_ => throw new NotSupportedException($"Unsupported texture format '{key.Format}'.")
		};

		if (!key.Data.IsEmpty && key.Data.Length != checked(pixels * channels))
			throw new ArgumentException("Texture data does not match its dimensions and format.", nameof(key));

		Texture texture = device.Device.CreateTexture(new WebGpuSharp.TextureDescriptor
		{
			Size = new(key.Width, key.Height, 1),
			Dimension = TextureDimension.D2,
			Format = WebGpuSharp.TextureFormat.RGBA8Unorm,
			Usage = TextureUsage.TextureBinding | TextureUsage.CopyDst
		});

		if (!key.Data.IsEmpty)
		{
			ReadOnlySpan<byte> data = key.Data;
			if (channels == 3)
			{
				byte[] rgba = new byte[checked(pixels * 4)];
				for (int i = 0; i < pixels; i++)
				{
					data.Slice(i * 3, 3).CopyTo(rgba.AsSpan(i * 4, 3));
					rgba[i * 4 + 3] = 255;
				}
				data = rgba;
			}
			device.Queue.WriteTexture(new TexelCopyTextureInfo { Texture = texture }, data, new TexelCopyBufferLayout { BytesPerRow = checked(key.Width * 4), RowsPerImage = key.Height }, new Extent3D(key.Width, key.Height, 1));
		}

		return new WebGPUTexture(texture);
	}

	/// <inheritdoc/>
	public ITextureView CreateTextureView(TextureViewDescriptor key) => new WebGPUTextureView(((WebGPUTexture)key.Texture).Resource.CreateView());

	/// <inheritdoc/>
	public ISampler CreateSampler(SamplerDescriptor key) => new WebGPUSampler(device.Device.CreateSampler(new WebGpuSharp.SamplerDescriptor
	{
		MinFilter = key.Sampler.Filter == Resources.SamplerFilter.Nearest ? FilterMode.Nearest : FilterMode.Linear,
		MagFilter = key.Sampler.Filter == Resources.SamplerFilter.Nearest ? FilterMode.Nearest : FilterMode.Linear,
		MipmapFilter = MipmapFilterMode.Nearest,
		AddressModeU = key.Sampler.WrapU == TextureWrapMode.Clamp ? AddressMode.ClampToEdge : AddressMode.Repeat,
		AddressModeV = key.Sampler.WrapV == TextureWrapMode.Clamp ? AddressMode.ClampToEdge : AddressMode.Repeat
	}) ?? throw new InvalidOperationException("Failed to create WebGPU sampler."));

	/// <inheritdoc/>
	public IResourceLayout CreateResourceLayout(ResourceLayoutDescriptor key)
	{
		BindGroupLayoutEntry[] entries = new BindGroupLayoutEntry[key.Elements.Length];
		for (int i = 0; i < entries.Length; i++)
		{
			var element = key.Elements[i];
			var entry = new BindGroupLayoutEntry
			{
				Binding = element.Binding ?? (uint)i,
				Visibility = ToWebGPU(element.Stages),
			};
			switch (element.Kind)
			{
				case ResourceKind.UniformBuffer:
					entry.Buffer = new() { Type = BufferBindingType.Uniform };
					break;
				case ResourceKind.TextureReadOnly:
					entry.Texture = new() { SampleType = TextureSampleType.Float, ViewDimension = TextureViewDimension.D2 };
					break;
				case ResourceKind.Sampler:
					entry.Sampler = new() { Type = SamplerBindingType.Filtering };
					break;
				default:
					throw new NotSupportedException($"Unsupported resource kind '{element.Kind}'.");
			}
			entries[i] = entry;
		}

		return new WebGPUResourceLayout(device.Device.CreateBindGroupLayout(new BindGroupLayoutDescriptor { Entries = entries }), entries.Select(e => e.Binding).ToArray());
	}

	/// <inheritdoc/>
	public IResourceSet CreateResourceSet(ResourceDescriptor key)
	{
		var layout = (WebGPUResourceLayout)key.Layout;
		if (key.BoundResources.Length != layout.Bindings.Length)
			throw new ArgumentException("Resource count does not match the layout.", nameof(key));

		BindGroupEntry[] entries = new BindGroupEntry[key.BoundResources.Length];
		for (int i = 0; i < entries.Length; i++)
		{
			entries[i] = key.BoundResources[i] switch
			{
				WebGPUBuffer buffer => new() { Binding = layout.Bindings[i], Buffer = buffer.Resource, Size = buffer.SizeInBytes },
				WebGPUTextureView texture => new() { Binding = layout.Bindings[i], TextureView = texture.Resource },
				WebGPUSampler sampler => new() { Binding = layout.Bindings[i], Sampler = sampler.Resource },
				_ => throw new NotSupportedException($"Unsupported bound resource '{key.BoundResources[i].GetType().Name}'.")
			};
		}

		return new WebGPUResourceSet(device.Device.CreateBindGroup(new BindGroupDescriptor { Layout = layout.Resource, Entries = entries }));
	}

	/// <inheritdoc/>
	public IPipeline CreateGraphicsPipeline(PipelineDescriptor key)
	{
		var output = (WebGPUFramebuffer)key.Output;
		var vertex = (WebGPUShader)CreateShader(key.VertShader);
		var fragment = (WebGPUShader)CreateShader(key.FragShader);
		return new WebGPUPipeline(device.Device.CreateRenderPipelineSync(new RenderPipelineDescriptor
		{
			Layout = CreatePipelineLayout(key.ResourceGroups),
			Vertex = new()
			{
				Module = vertex.Resource,
				Buffers = [new VertexBufferLayout
				{
					ArrayStride = 32,
					StepMode = VertexStepMode.Vertex,
					Attributes = [
						new VertexAttribute { ShaderLocation = 0, Offset = 0, Format = VertexFormat.Float32x3 },
						new VertexAttribute { ShaderLocation = 1, Offset = 12, Format = VertexFormat.Float32x3 },
						new VertexAttribute { ShaderLocation = 2, Offset = 24, Format = VertexFormat.Float32x2 }]
				}]
			},
			Fragment = new() { Module = fragment.Resource, Targets = [new ColorTargetState { Format = output.Format, WriteMask = ColorWriteMask.All }] },
			Primitive = new()
			{
				Topology = PrimitiveTopology.TriangleList,
				FrontFace = FrontFace.CW,
				CullMode = key.CullingMode switch { CullingMode.Back => CullMode.Back, CullingMode.Front => CullMode.Front, _ => CullMode.None }
			},
			DepthStencil = new()
			{
				Format = WebGpuSharp.TextureFormat.Depth32Float,
				DepthWriteEnabled = key.DepthInfo.WriteDepth ? OptionalBool.True : OptionalBool.False,
				DepthCompare = ToWebGPU(key.DepthInfo.DepthComparison)
			}
		}));
	}

	/// <inheritdoc/>
	public IComputePipeline CreateComputePipeline(ComputePipelineDescriptor key)
	{
		var shader = (WebGPUShader)CreateShader(key.Shader);
		return new WebGPUComputePipeline(device.Device.CreateComputePipelineSync(new WebGpuSharp.ComputePipelineDescriptor
		{
			Layout = CreatePipelineLayout(key.ResourceGroups),
			Compute = new() { Module = shader.Resource }
		}));
	}

	private PipelineLayout CreatePipelineLayout(ReadOnlySpan<ShaderResourceGroup> groups)
	{
		int count = 0;
		foreach (var group in groups)
			count = System.Math.Max(count, checked((int)group.Set + 1));

		BindGroupLayout[] layouts = new BindGroupLayout[count];
		foreach (var group in groups)
		{
			if (layouts[group.Set] != null)
				throw new ArgumentException("Duplicate resource set index.", nameof(groups));

			layouts[group.Set] = ((WebGPUResourceLayout)CreateResourceLayout(new ResourceLayoutDescriptor(group.LayoutElements.AsSpan()))).Resource;
		}

		for (int i = 0; i < count; i++)
			layouts[i] ??= device.Device.CreateBindGroupLayout(new BindGroupLayoutDescriptor { Entries = [] });

		return device.Device.CreatePipelineLayout(new PipelineLayoutDescriptor { BindGroupLayouts = layouts });
	}

	private static ShaderStage ToWebGPU(Resources.ShaderStages stages)
	{
		ShaderStage result = 0;
		if (stages.HasFlag(Resources.ShaderStages.Vertex))
			result |= ShaderStage.Vertex;

		if (stages.HasFlag(Resources.ShaderStages.Fragment))
			result |= ShaderStage.Fragment;

		if (stages.HasFlag(Resources.ShaderStages.Compute))
			result |= ShaderStage.Compute;

		return result;
	}

	private static CompareFunction ToWebGPU(ComparisonMode comparison) => comparison switch
	{
		ComparisonMode.Always => CompareFunction.Always,
		ComparisonMode.Less => CompareFunction.Less,
		ComparisonMode.LessOrEqual => CompareFunction.LessEqual,
		ComparisonMode.Equal => CompareFunction.Equal,
		ComparisonMode.NotEqual => CompareFunction.NotEqual,
		ComparisonMode.GreaterOrEqual => CompareFunction.GreaterEqual,
		ComparisonMode.Greater => CompareFunction.Greater,
		ComparisonMode.Never => CompareFunction.Never,
		_ => throw new ArgumentOutOfRangeException(nameof(comparison))
	};
}
