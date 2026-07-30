// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI;
using Beryl.Rendering.Resources;

using Silk.NET.Windowing;


namespace Beryl.Rendering;

public static class Renderer
{
	/// <summary> The options for the renderer. </summary>
	public static RendererOptions Options { get; private set; }

	/// <summary> The renderer backend in use. </summary>
	public static RendererBackend Backend { get; private set; }

	/// <summary> The lower-level <see cref="IGraphicsDevice"/>. </summary>
	public static IGraphicsDevice Device { get; private set; } = null!;

	/// <summary> Incremented every rendered frame. </summary>
	public static ulong CurrentFrame { get; private set; }

	/// <summary> Initializes the desired backend. Should be called once after window loading. </summary>
	/// <param name="window"> The window to initialize the backend for. </param>
	/// <param name="backend"> The backend to initialize. </param>
	public static void Initialize(IWindow window, RendererBackend? backend = null, RendererOptions options = default)
	{
		Options = options;

		if (backend is null)
		{
			backend = GraphicsDeviceFactory.IsBackendSupported(RendererBackend.Vulkan)
				? RendererBackend.Vulkan
				: GraphicsDeviceFactory.IsBackendSupported(RendererBackend.Direct3D12)
					? RendererBackend.Direct3D12
					: throw new InvalidOperationException("No supported graphics backend found.");
		}

		Backend = backend.Value;

		Device = GraphicsDeviceFactory.Create(Backend);
		Device.Initialize(window);
	}

	/// <summary> Presents the current frame. </summary>
	public static void Present()
	{
		Device.SwapBuffers();
		CurrentFrame++;

		FrameDisposalQueue.DisposeResources(CurrentFrame);
	}

	/// <summary> Submits the given <see cref="ICommandBuffer"/> to be ran. </summary>
	public static void SubmitCommandBuffer(ICommandBuffer commandBuffer) => Device.SubmitCommands(commandBuffer);

	/// <summary> Resizes the renderer. </summary>
	public static void Resize(uint width, uint height) => Device.ResizeSwapchain(width, height);

	/// <summary> Cleans up the renderer. </summary>
	public static void Dispose()
	{
		FrameDisposalQueue.DisposeResources(ulong.MaxValue);
		Device.Dispose();
	}
}
