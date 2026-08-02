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

	/// <inheritdoc/>
	public override void Initialize()
	{
		Application.OnResize += (size) => OnResize((int)size.X, (int)size.Y);
		Application.OnRender += Render;

		Renderer.Initialize(Application.Window.InternalWindow, RendererBackend.Vulkan);
		Application.Window.Title += $" - [{Renderer.Backend}]";
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

		Renderer.Present();
	}

	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();

		Renderer.Dispose();

		Application.OnResize -= (size) => OnResize((int)size.X, (int)size.Y);
		Application.OnRender -= Render;
	}

	/// <summary> Renders a client renderable to the window during the next frame. </summary>
	/// <param name="renderable">The object to render.</param>
	/// <param name="transform">The object's transform matrix.</param>
	public void QueueObject(IClientRenderable renderable, Matrix4x4 transform) => Renderables.Add((renderable, transform));

	public void OnResize(int width, int height) => Renderer.Resize((uint)width, (uint)height);
}
