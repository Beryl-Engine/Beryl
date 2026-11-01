// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Collections.Immutable;

using Beryl.Common;
using Beryl.Math;
using Beryl.Rendering;
using Beryl.Rendering.Resources;
using Beryl.RHI.Resources;

namespace Beryl.GUI;

/// <summary>
/// Handles low-level rendering of GUI elements.
/// </summary>
public class PaintEngine
{
	private Shader? rectShader;

	/// <summary> Framebuffer pixels per logical unit, set by GUIModule for DPI and user scaling. </summary>
	public Vector2 CoordinateScale { get; set; } = Vector2.One;

	/// <summary> Queues a rectangle in logical coordinates. </summary>
	public void DrawRect(Rect rect, Color color)
	{
		if (rect.Size.X <= 0 || rect.Size.Y <= 0 || color.A <= 0)
			return;

		var viewport = Application.Window.InternalWindow.FramebufferSize;
		if (viewport.X <= 0 || viewport.Y <= 0)
			return;

		var rendering = ModuleManager.GetModule<RenderingModule>();
		rectShader ??= Application.ResourceProvider.GetResource<Shader>("Assets/Defaults/Rect.slang");

		if (rectShader == null || rendering == null)
			return;

		Material material = new(rectShader);
		material.SetProperty("Color", new ShaderValue.Vector(color.R, color.G, color.B, color.A));
		material.SetProperty("RectPosition", new ShaderValue.Vector(rect.Position.X * CoordinateScale.X, rect.Position.Y * CoordinateScale.Y));
		material.SetProperty("RectSize", new ShaderValue.Vector(rect.Size.X * CoordinateScale.X, rect.Size.Y * CoordinateScale.Y));
		material.SetProperty("ViewportSize", new ShaderValue.Vector(viewport.X, viewport.Y));

		rendering.QueueObject(new RectRenderable(material), Matrix4x4.Identity);
	}

	private sealed class RectRenderable(Material material) : IClientRenderable
	{
		private static readonly ImmutableArray<Vector3> vertices = [new(0, 0, 0), new(1, 0, 0), new(1, 1, 0), new(0, 1, 0)];
		private static readonly ImmutableArray<uint> indices = [0, 1, 2, 0, 2, 3];

		public ImmutableArray<Vector3> Vertices => vertices;
		public ImmutableArray<Vector3> Normals => [];
		public ImmutableArray<Vector2> UVs => [];
		public ImmutableArray<uint> Indices => indices;
		public Material Material { get; } = material;
	}
}
