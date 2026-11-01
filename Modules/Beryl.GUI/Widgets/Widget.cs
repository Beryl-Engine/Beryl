// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.GUI.Layouts;
using Beryl.Math;

namespace Beryl.GUI.Widgets;

/// <summary>
/// Base class for all GUI widgets.
/// </summary>
public class Widget
{
	/// <summary> The parent <see cref="Widget"/>. </summary>
	public Widget? Parent { get; internal set; }

	/// <summary> Requested local bounds in logical units. </summary>
	public Rect Rect { get; set; }

	/// <summary> The <see cref="Color"/> to draw the background with. </summary>
	public Color BackgroundColor { get; set; } = Color.White;

	/// <summary> The bounds produced by the <see cref="Layout"/>. </summary>
	public Rect Bounds { get; private set; }

	public Thickness Padding { get; set; }
	public Vector2 MinimumSize { get; set; }
	public Vector2 PreferredSize { get; set; }
	public Vector2 MaximumSize { get; set; } = new(float.PositiveInfinity, float.PositiveInfinity);
	public Vector2 DesiredSize { get; private set; }

	/// <summary> The <see cref="Layouts.Layout"/> that drives <see cref="Widget"/> positioning and sizing. </summary>
	public Layout? Layout
	{
		get => field;
		set
		{
			value?.Attach(this);
			field?.Detach();
			field = value;
		}
	}

	/// <summary> All child <see cref="Widget"/>s. </summary>
	public WidgetCollection Children { get; }

	public Widget() => Children = new(this);

	/// <summary> Called when the <see cref="Widget"/> is updated. </summary>
	public virtual void Update() { }

	/// <summary> Called when the <see cref="Widget"/> requests a draw. </summary>
	public virtual void Draw(PaintEngine pEngine) => pEngine.DrawRect(Bounds, BackgroundColor);

	/// <summary> Computes the desired size. </summary>
	public void Measure(Vector2 availableSize)
	{
		Vector2 inner = new(Math.Math.Max(0, availableSize.X - Padding.Size.X), Math.Math.Max(0, availableSize.Y - Padding.Size.Y));
		Vector2 content = MeasureContent(inner);

		if (Layout != null)
		{
			var layoutSize = Layout.Measure(inner);
			content = new(Math.Math.Max(content.X, layoutSize.X), Math.Math.Max(content.Y, layoutSize.Y));
		}
		else
		{
			foreach (var child in Children)
			{
				child.Measure(inner);
				content = new(Math.Math.Max(content.X, child.Rect.Position.X + child.DesiredSize.X), Math.Math.Max(content.Y, child.Rect.Position.Y + child.DesiredSize.Y));
			}
		}

		DesiredSize = Constrain(new(Math.Math.Max(Math.Math.Max(PreferredSize.X, Rect.Size.X), content.X + Padding.Size.X), Math.Math.Max(Math.Math.Max(PreferredSize.Y, Rect.Size.Y), content.Y + Padding.Size.Y)));
	}

	protected virtual Vector2 MeasureContent(Vector2 availableSize) => Vector2.Zero;

	/// <summary> Assigns bounds and recursively arranges children. </summary>
	public void Arrange(Rect bounds)
	{
		Bounds = bounds;
		Rect content = Padding.Deflate(bounds);

		if (Layout != null)
		{
			Layout.UpdateGeometry(content);
		}
		else
		{
			foreach (var child in Children)
			{
				child.Arrange(new(content.Position.X + child.Rect.Position.X, content.Position.Y + child.Rect.Position.Y,
					child.DesiredSize.X, child.DesiredSize.Y));
			}
		}
	}

	internal Vector2 Constrain(Vector2 size)
	{
		if (!float.IsFinite(MinimumSize.X) || !float.IsFinite(MinimumSize.Y) || MinimumSize.X < 0 || MinimumSize.Y < 0 || float.IsNaN(MaximumSize.X) || float.IsNaN(MaximumSize.Y) || MaximumSize.X < MinimumSize.X || MaximumSize.Y < MinimumSize.Y)
			throw new InvalidOperationException("Widget size limits must be nonnegative and maximum must be at least minimum.");

		return new(System.Math.Clamp(size.X, MinimumSize.X, MaximumSize.X), System.Math.Clamp(size.Y, MinimumSize.Y, MaximumSize.Y));
	}

	internal void UpdateTree()
	{
		Update();
		foreach (var child in Children)
			child.UpdateTree();
	}

	internal void DrawTree(PaintEngine engine)
	{
		Draw(engine);
		foreach (var child in Children)
			child.DrawTree(engine);
	}
}
