// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI;
using Beryl.RHI.Resources;

namespace Beryl.Rendering.Pipelines;

/// <summary>
/// Render instructions for a step in the rendering pipeline.
/// </summary>
public abstract class RenderPass
{
	/// <summary> The <see cref="RenderPipeline"/> that owns this <see cref="RenderPass"/>. </summary>
	public RenderPipeline Pipeline { get; init; } = null!;

	/// <summary> Fully runs this <see cref="RenderPass"/>. </summary>
	public void Execute(in RenderFrame frame)
	{
		using var buffer = CommandBufferPool.Shared.RentAuto();
		buffer.Object.Begin();

		buffer.Object.SetFrameBuffer(TargetBuffer);
		buffer.Object.SetConstantBuffer("Camera", frame.CameraBuffer);

		Render(in frame, buffer.Object);

		buffer.Object.End();
		Renderer.SubmitCommandBuffer(buffer.Object);
	}

	/// <summary> Performs rendering operations. </summary>
	/// <remarks> This method is called by <see cref="RenderPass.Execute(in RenderFrame)"/>. </remarks>
	public abstract void Render(in RenderFrame frame, ICommandBuffer buffer);

	/// <summary> The target <see cref="IFramebuffer"/> this pass is rendering to. </summary>
	protected virtual IFramebuffer TargetBuffer => Renderer.Device.SwapchainFramebuffer;
}
