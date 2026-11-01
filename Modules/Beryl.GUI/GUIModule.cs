// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.GUI.Layouts;
using Beryl.GUI.Widgets;
using Beryl.Math;

namespace Beryl.GUI;

/// <summary>
/// Module responsible for GUI.
/// </summary>
public class GUIModule : BaseModule
{
	/// <summary> The <see cref="GUI.PaintEngine"/> used to render <see cref="Widget"/>s. </summary>
	public PaintEngine PaintEngine { get; set; } = new();

	/// <summary> The root widget. </summary>
	/// <remarks> This fills the entire viewport. </remarks>
	public Widget RootWidget { get; set; } = new() { BackgroundColor = Color.Transparent, Layout = new AnchorLayout(AnchorLayout.AnchorPoint.Center) };

	/// <summary> User scale applied in addition to the window's DPI scaling. </summary>
	public float UIScale { get; set; } = 1;

	/// <summary> Measures and arranges the tree. </summary>
	public void UpdateLayout(Vector2 logicalViewport)
	{
		if (!float.IsFinite(logicalViewport.X) || !float.IsFinite(logicalViewport.Y) || logicalViewport.X < 0 || logicalViewport.Y < 0)
			throw new ArgumentOutOfRangeException(nameof(logicalViewport));

		RootWidget.Measure(logicalViewport);
		RootWidget.Arrange(new(0, 0, logicalViewport.X, logicalViewport.Y));
	}

	/// <inheritdoc/>
	public override void Initialize()
	{
		Application.OnUpdate += Update;
		Widget widget = new()
		{
			Rect = new(0, 0, 16, 16)
		};

		RootWidget.Children.Add(widget);
	}

	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();
		Application.OnUpdate -= Update;
	}

	public void Update()
	{
		RootWidget.UpdateTree();

		var window = Application.Window;
		var framebuffer = window.InternalWindow.FramebufferSize;
		var size = window.Size;
		if (size.X <= 0 || size.Y <= 0 || framebuffer.X <= 0 || framebuffer.Y <= 0)
			return;

		Vector2 logical = new(size.X / UIScale, size.Y / UIScale);
		PaintEngine.CoordinateScale = new(framebuffer.X / logical.X, framebuffer.Y / logical.Y);
		UpdateLayout(logical);
		RootWidget.DrawTree(PaintEngine);
	}
}
