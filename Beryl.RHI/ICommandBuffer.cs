// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;
using Beryl.RHI.Resources;

namespace Beryl.RHI;

/// <summary>
/// Low-level buffer of GPU commands to be submitted.
/// </summary>
public interface ICommandBuffer : IDisposable
{
	#region Lifecycle
	/// <summary> Begins recording GPU commands. </summary>
	void Begin();

	/// <summary> Ends recording GPU commands. </summary>
	void End();
	#endregion

	#region Render Targets
	/// <summary> Sets the <see cref="IFramebuffer"/> to render commands to. </summary>
	void SetFrameBuffer(IFramebuffer frameBuffer);

	/// <summary> The currently set framebuffer target. </summary>
	IFramebuffer? CurrentFramebuffer { get; }
	#endregion

	#region Pipeline & Binding
	/// <summary> Sets the active pipeline. </summary>
	void SetPipeline(IPipeline pipeline);

	/// <summary> Sets the active compute pipeline. </summary>
	void SetComputePipeline(IComputePipeline pipeline);

	/// <summary> Sets a vertex buffer at the given slot. </summary>
	void SetVertexBuffer(uint slot, IBuffer buffer);

	/// <summary> Sets the index buffer. </summary>
	void SetIndexBuffer(IBuffer buffer);

	/// <summary> Binds a graphics resource set to the given slot. </summary>
	void SetGraphicsResourceSet(uint slot, IResourceSet resourceSet);

	/// <summary> Binds a compute resource set to the given slot. </summary>
	void SetComputeResourceSet(uint slot, IResourceSet resourceSet);
	#endregion

	#region Buffer Updates
	/// <summary> Updates a buffer with new data. </summary>
	void UpdateBuffer(IBuffer buffer, uint offset, ReadOnlySpan<byte> data);
	#endregion

	#region Drawing
	/// <summary> Draws indexed primitives. </summary>
	void DrawIndexed(uint indexCount);
	#endregion

	#region Dispatching
	/// <summary> Dispatches a compute job. </summary>
	void Dispatch(uint x, uint y, uint z);
	#endregion

	#region Clearing
	/// <summary> Clears the depth stencil to <paramref name="depth"/>. </summary>
	void ClearDepth(float depth);

	/// <summary> Clears the color buffer to <paramref name="color"/>. </summary>
	void ClearColor(Color color);
	#endregion
}
