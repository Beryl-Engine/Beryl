// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Runtime.InteropServices;

using Beryl.Common;
using Beryl.Math;
using Beryl.RHI;
using Beryl.Rendering.Components;
using Beryl.Rendering.Pipelines;
using Beryl.Rendering.Pipelines.Forward;
using Beryl.Rendering.Resources;

using Silk.NET.Windowing;

namespace Beryl.Rendering;

/// <summary>
/// Module responsible for rendering meshes to the window.
/// </summary>
public class RenderingModule : BaseModule
{
	/// <summary> The renderable objects to render next frame. </summary>
	internal List<(IClientRenderable Renderable, Matrix4x4 Transform)> Renderables { get; set; } = new();

	/// <summary> The <see cref="RenderPipeline"/> in use. </summary>
	public RenderPipeline Pipeline { get; set; } = new ForwardRenderPipeline();

	/// <summary> The renderer backend in use. </summary>
	public RendererBackend Backend { get; private set; }

	/// <summary> The lower-level <see cref="IGraphicsDevice"/>. </summary>
	public IGraphicsDevice Device { get; private set; } = null!;

	/// <summary> Incremented every rendered frame. </summary>
	public ulong CurrentFrame { get; private set; }

	/// <inheritdoc/>
	public override void Initialize()
	{
		Application.OnResize += (size) => OnResize((int)size.X, (int)size.Y);
		Application.OnRender += Render;

		InitializeDevice(Application.Window.InternalWindow, RendererBackend.Vulkan);
		Application.Window.Title += $" - [{Backend}]";
	}

	public void Render()
	{
		if (Camera.Main == null)
			new Camera();

		Matrix4x4 view = Camera.Main?.ViewMatrix ?? Matrix4x4.Identity;
		Matrix4x4 projection = Camera.Main?.ProjectionMatrix ?? Matrix4x4.Identity;

		ReadOnlySpan<byte> cameraBuffer = Shader.DefaultsProvider.GetCameraBuffer(Camera.Main!);

		var frame = new RenderFrame()
		{
			View = view,
			Projection = projection,
			CameraBuffer = cameraBuffer,
			Renderables = CollectionsMarshal.AsSpan(Renderables),
		};

		Pipeline.Render(in frame);
		Renderables.Clear();

		Present();
	}

	/// <summary> Presents the current frame. </summary>
	public void Present()
	{
		Device.SwapBuffers();
		CurrentFrame++;

		FrameDisposalQueue.DisposeResources(CurrentFrame);
	}

	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();

		DisposeDevice();

		Application.OnResize -= (size) => OnResize((int)size.X, (int)size.Y);
		Application.OnRender -= Render;
	}

	/// <summary> Renders a client renderable to the window during the next frame. </summary>
	/// <param name="renderable">The object to render.</param>
	/// <param name="transform">The object's transform matrix.</param>
	public void QueueObject(IClientRenderable renderable, Matrix4x4 transform) => Renderables.Add((renderable, transform));

	public void OnResize(int width, int height) => Resize((uint)width, (uint)height);

	/// <summary> Resizes the renderer. </summary>
	public void Resize(uint width, uint height) => Device.ResizeSwapchain(width, height);

	/// <summary> Submits the given <see cref="ICommandBuffer"/> to be ran. </summary>
	public void SubmitCommandBuffer(ICommandBuffer commandBuffer) => Device.SubmitCommands(commandBuffer);

	/// <summary> Initializes the desired backend. Should be called once after window loading. </summary>
	/// <param name="window"> The window to initialize the backend for. </param>
	/// <param name="backend"> The backend to initialize. </param>
	private void InitializeDevice(IWindow window, RendererBackend? backend = null)
	{
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

	/// <summary> Cleans up the renderer. </summary>
	private void DisposeDevice()
	{
		FrameDisposalQueue.DisposeResources(ulong.MaxValue);
		Device.Dispose();
	}
}
