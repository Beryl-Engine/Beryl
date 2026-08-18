// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;
using Beryl.RHI.Resources;
using Silk.NET.Vulkan;

namespace Beryl.RHI.VulkanBackend;

internal sealed class VulkanCommandBuffer(Vk vk) : ICommandBuffer
{
	/// <inheritdoc/>
	public IFramebuffer? CurrentFramebuffer => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Begin() => throw new NotImplementedException();

	/// <inheritdoc/>
	public void End() => throw new NotImplementedException();

	/// <inheritdoc/>
	public void ClearColor(Color color) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void ClearDepth(float depth) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void DrawIndexed(uint indexCount) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SetFrameBuffer(IFramebuffer frameBuffer) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SetGraphicsResourceSet(uint slot, IResourceSet resourceSet) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SetIndexBuffer(IBuffer buffer) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SetPipeline(IPipeline pipeline) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SetVertexBuffer(uint slot, IBuffer buffer) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void UpdateBuffer(IBuffer buffer, uint offset, ReadOnlySpan<byte> data) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Dispose() => throw new NotImplementedException();
}
