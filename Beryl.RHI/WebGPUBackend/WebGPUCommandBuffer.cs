// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;
using WebGpuSharp;

namespace Beryl.RHI.WebGPUBackend;

internal sealed class WebGPUCommandBuffer(WebGPUDevice device) : ICommandBuffer
{
	private CommandEncoder encoder;
	private WebGpuSharp.CommandBuffer commands;
	private RenderPassEncoder renderPass;
	private ComputePassEncoder computePass;
	private bool rendering;
	private bool computing;
	private bool recording;
	private bool finished;
	private bool disposed;
	private WebGPUFramebuffer? target;
	private WebGPUPipeline? pipeline;
	private WebGPUComputePipeline? computePipeline;
	private WebGPUBuffer? indexBuffer;
	private readonly Dictionary<uint, WebGPUBuffer> vertexBuffers = [];
	private readonly Dictionary<uint, WebGPUResourceSet> graphicsSets = [];
	private readonly Dictionary<uint, WebGPUResourceSet> computeSets = [];
	private readonly List<UploadBuffer> uploadBuffers = [];
	private int uploadBufferIndex;

	/// <inheritdoc/>
	public IFramebuffer? CurrentFramebuffer => target;

	/// <inheritdoc/>
	public void Begin()
	{
		ObjectDisposedException.ThrowIf(disposed, this);
		if (recording)
			throw new InvalidOperationException("The command buffer is already recording.");

		commands = default;
		finished = false;
		target = null;
		pipeline = null;
		computePipeline = null;
		indexBuffer = null;
		vertexBuffers.Clear();
		graphicsSets.Clear();
		computeSets.Clear();
		uploadBufferIndex = 0;

		foreach (var upload in uploadBuffers)
			upload.Length = 0;

		encoder = device.Device.CreateCommandEncoder();
		recording = true;
	}

	/// <inheritdoc/>
	public void End()
	{
		EnsureRecording();
		EndPass();

		commands = encoder.Finish();
		encoder = default;
		recording = false;
		finished = true;
	}

	internal void Submit()
	{
		ObjectDisposedException.ThrowIf(disposed, this);
		if (!finished)
			throw new InvalidOperationException("End the command buffer before submitting it.");

		foreach (var upload in uploadBuffers)
		{
			if (upload.Length != 0)
				device.Queue.WriteBuffer(upload.Resource, 0, upload.Data.AsSpan(0, upload.Length));
		}

		device.Queue.Submit(commands);
		commands = default;
		finished = false;
	}

	/// <inheritdoc/>
	public void SetFrameBuffer(IFramebuffer frameBuffer)
	{
		EnsureRecording();
		EndPass();

		target = (WebGPUFramebuffer)frameBuffer;
	}

	/// <inheritdoc/>
	public void SetPipeline(IPipeline pipeline)
	{
		EnsureRecording();

		this.pipeline = (WebGPUPipeline)pipeline;
		if (rendering)
			renderPass.SetPipeline(this.pipeline.Resource);
	}

	/// <inheritdoc/>
	public void SetComputePipeline(IComputePipeline pipeline)
	{
		EnsureRecording();

		computePipeline = (WebGPUComputePipeline)pipeline;
		if (computing)
			computePass.SetPipeline(computePipeline.Resource);
	}

	/// <inheritdoc/>
	public void SetVertexBuffer(uint slot, IBuffer buffer)
	{
		EnsureRecording();

		vertexBuffers[slot] = (WebGPUBuffer)buffer;
		if (rendering)
			renderPass.SetVertexBuffer(slot, vertexBuffers[slot].Resource, 0);
	}

	/// <inheritdoc/>
	public void SetIndexBuffer(IBuffer buffer)
	{
		EnsureRecording();

		indexBuffer = (WebGPUBuffer)buffer;
		if (rendering)
			renderPass.SetIndexBuffer(indexBuffer.Resource, IndexFormat.Uint32, 0);
	}

	/// <inheritdoc/>
	public void SetGraphicsResourceSet(uint slot, IResourceSet resourceSet)
	{
		EnsureRecording();

		graphicsSets[slot] = (WebGPUResourceSet)resourceSet;
		if (rendering)
			renderPass.SetBindGroup(slot, graphicsSets[slot].Resource);
	}

	/// <inheritdoc/>
	public void SetComputeResourceSet(uint slot, IResourceSet resourceSet)
	{
		EnsureRecording();

		computeSets[slot] = (WebGPUResourceSet)resourceSet;
		if (computing)
			computePass.SetBindGroup(slot, computeSets[slot].Resource);
	}

	/// <inheritdoc/>
	public void UpdateBuffer(IBuffer buffer, uint offset, ReadOnlySpan<byte> data)
	{
		EnsureRecording();

		var destination = (WebGPUBuffer)buffer;
		WebGPUDevice.ValidateBufferUpdate(destination, offset, data.Length);
		if (data.IsEmpty)
			return;

		EndPass();
		var upload = GetUploadBuffer(data.Length);
		int sourceOffset = upload.Length;
		data.CopyTo(upload.Data.AsSpan(sourceOffset));
		upload.Length += data.Length;
		encoder.CopyBufferToBuffer(upload.Resource, (ulong)sourceOffset, destination.Resource, offset, (ulong)data.Length);
	}

	private UploadBuffer GetUploadBuffer(int length)
	{
		while (uploadBufferIndex < uploadBuffers.Count)
		{
			var upload = uploadBuffers[uploadBufferIndex];
			if (upload.Data.Length - upload.Length >= length)
				return upload;
			uploadBufferIndex++;
		}

		int capacity = System.Math.Max(64 * 1024, length);
		var resource = device.Device.CreateBuffer(new WebGpuSharp.BufferDescriptor
		{
			Size = (ulong)capacity,
			Usage = WebGpuSharp.BufferUsage.CopySrc | WebGpuSharp.BufferUsage.CopyDst
		});
		var buffer = new UploadBuffer(resource, capacity);
		uploadBuffers.Add(buffer);
		return buffer;
	}

	/// <inheritdoc/>
	public void DrawIndexed(uint indexCount)
	{
		EnsureRecording();
		if (pipeline == null || indexBuffer == null)
			throw new InvalidOperationException("Indexed draws require a graphics pipeline and index buffer.");

		BeginRenderPass();
		renderPass.DrawIndexed(indexCount, 1, 0, 0, 0);
	}

	/// <inheritdoc/>
	public void Dispatch(uint x, uint y, uint z)
	{
		EnsureRecording();
		if (computePipeline == null)
			throw new InvalidOperationException("Dispatch requires a compute pipeline.");

		if (!computing)
		{
			EndPass();
			computePass = encoder.BeginComputePass();
			computing = true;
			computePass.SetPipeline(computePipeline.Resource);
			foreach (var (slot, set) in computeSets)
				computePass.SetBindGroup(slot, set.Resource);
		}

		computePass.DispatchWorkgroups(x, y, z);
	}

	/// <inheritdoc/>
	public void ClearDepth(float depth)
	{
		EnsureRecording();
		EndPass();
		BeginRenderPass(depth: depth);
	}

	/// <inheritdoc/>
	public void ClearColor(Beryl.Math.Color color)
	{
		EnsureRecording();
		EndPass();
		BeginRenderPass(color: new WebGpuSharp.Color(color.R, color.G, color.B, color.A));
	}

	private void BeginRenderPass(WebGpuSharp.Color? color = null, float? depth = null)
	{
		if (rendering)
			return;

		if (target == null)
			throw new InvalidOperationException("A framebuffer must be set before rendering.");

		EndPass();

		renderPass = encoder.BeginRenderPass(new RenderPassDescriptor
		{
			ColorAttachments = [new RenderPassColorAttachment
			{
				View = target.ColorView,
				LoadOp = color.HasValue ? LoadOp.Clear : LoadOp.Load,
				StoreOp = StoreOp.Store,
				ClearValue = color.GetValueOrDefault()
			}],
			DepthStencilAttachment = new RenderPassDepthStencilAttachment
			{
				View = target.DepthView,
				DepthLoadOp = depth.HasValue ? LoadOp.Clear : LoadOp.Load,
				DepthStoreOp = StoreOp.Store,
				DepthClearValue = depth.GetValueOrDefault(1)
			}
		});

		rendering = true;
		if (pipeline != null)
			renderPass.SetPipeline(pipeline.Resource);

		foreach (var (slot, buffer) in vertexBuffers)
			renderPass.SetVertexBuffer(slot, buffer.Resource, 0);

		if (indexBuffer != null)
			renderPass.SetIndexBuffer(indexBuffer.Resource, IndexFormat.Uint32, 0);

		foreach (var (slot, set) in graphicsSets)
			renderPass.SetBindGroup(slot, set.Resource);
	}

	private void EndPass()
	{
		if (rendering)
			renderPass.End();

		if (computing)
			computePass.End();

		rendering = false;
		computing = false;
		renderPass = default;
		computePass = default;
	}

	private void EnsureRecording()
	{
		ObjectDisposedException.ThrowIf(disposed, this);
		if (!recording)
			throw new InvalidOperationException("Begin the command buffer before recording commands.");
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		if (disposed)
			return;

		EndPass();

		commands = default;
		encoder = default;
		target = null;
		pipeline = null;
		computePipeline = null;
		indexBuffer = null;
		vertexBuffers.Clear();
		graphicsSets.Clear();
		computeSets.Clear();

		foreach (var upload in uploadBuffers)
			upload.Resource.Destroy();

		uploadBuffers.Clear();
		recording = false;
		finished = false;
		disposed = true;
	}

	private sealed class UploadBuffer(WebGpuSharp.Buffer resource, int capacity)
	{
		internal WebGpuSharp.Buffer Resource { get; } = resource;
		internal byte[] Data { get; } = new byte[capacity];
		internal int Length { get; set; }
	}
}
