// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;
using Beryl.RHI.Resources;
using Veldrith;

namespace Beryl.RHI.VeldrithBackend;

/// <summary>
/// Veldrith-based implementation of <see cref="ICommandBuffer"/>.
/// </summary>
internal sealed class VeldrithCommandBuffer : ICommandBuffer
{
	public CommandList CommandList { get; }

	private IFramebuffer? target;

	/// <inheritdoc/>
	public IFramebuffer? CurrentFramebuffer => target;

	/// <summary> Initializes a new instance of the <see cref="VeldrithCommandBuffer"/> class. </summary>
	internal VeldrithCommandBuffer(VeldrithDevice device) => CommandList = device.GraphicsDevice.ResourceFactory.CreateCommandList();

	/// <inheritdoc/>
	public void Dispose() => CommandList.Dispose();

	/// <inheritdoc/>
	public void Begin() => CommandList.Begin();

	/// <inheritdoc/>
	public void End() => CommandList.End();

	/// <inheritdoc/>
	public void SetPipeline(IPipeline pipeline) => CommandList.SetPipeline(((VeldrithPipeline)pipeline).Resource);

	/// <inheritdoc/>
	public void SetComputePipeline(IComputePipeline pipeline) => CommandList.SetPipeline(((VeldrithPipeline)pipeline).Resource);

	/// <inheritdoc/>
	public void SetVertexBuffer(uint slot, IBuffer buffer) => CommandList.SetVertexBuffer(slot, ((VeldrithBuffer)buffer).Resource);

	/// <inheritdoc/>
	public void SetIndexBuffer(IBuffer buffer) => CommandList.SetIndexBuffer(((VeldrithBuffer)buffer).Resource, IndexFormat.UInt32);

	/// <inheritdoc/>
	public void SetGraphicsResourceSet(uint slot, IResourceSet resourceSet) => CommandList.SetGraphicsResourceSet(slot, ((VeldrithResourceSet)resourceSet).Resource);

	/// <inheritdoc/>
	public void SetComputeResourceSet(uint slot, IResourceSet resourceSet) => CommandList.SetComputeResourceSet(slot, ((VeldrithResourceSet)resourceSet).Resource);

	/// <inheritdoc/>
	public void UpdateBuffer(IBuffer buffer, uint offset, ReadOnlySpan<byte> data) => CommandList.UpdateBuffer(((VeldrithBuffer)buffer).Resource, offset, data);

	/// <inheritdoc/>
	public void DrawIndexed(uint indexCount) => CommandList.DrawIndexed(indexCount);

	/// <inheritdoc/>
	public void Dispatch(uint x, uint y, uint z) => CommandList.Dispatch(x, y, z);

	/// <inheritdoc/>
	public void SetFrameBuffer(IFramebuffer frameBuffer)
	{
		target = frameBuffer;
		CommandList.SetFramebuffer(((VeldrithFramebuffer)frameBuffer).Framebuffer);
	}

	/// <inheritdoc/>
	public void ClearDepth(float depth) => CommandList.ClearDepthStencil(depth);

	/// <inheritdoc/>
	public void ClearColor(Color color) => CommandList.ClearColorTarget(0, new RgbaFloat(color.R, color.G, color.B, color.A));
}
