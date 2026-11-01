// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;

namespace Beryl.GUI.Layouts;

public class AnchorLayout : Layout
{
	public enum AnchorPoint
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
		Center
	}

	/// <summary> The point used to align children. </summary>
	public AnchorPoint Point { get; set; }

	public AnchorLayout() { }
	public AnchorLayout(AnchorPoint point) => Point = point;

	/// <inheritdoc/>
	public override Vector2 Measure(Vector2 availableSize)
	{
		Vector2 desired = Vector2.Zero;
		foreach (var child in Widgets)
		{
			child.Measure(availableSize);
			desired = new(Math.Math.Max(desired.X, child.DesiredSize.X), Math.Math.Max(desired.Y, child.DesiredSize.Y));
		}

		return desired;
	}

	/// <inheritdoc/>
	public override void UpdateGeometry(Rect rect)
	{
		foreach (var child in Widgets)
		{
			var size = child.DesiredSize;
			Vector2 offset = Point switch
			{
				AnchorPoint.TopRight => new(rect.Size.X - size.X, 0),
				AnchorPoint.BottomLeft => new(0, rect.Size.Y - size.Y),
				AnchorPoint.BottomRight => rect.Size - size,
				AnchorPoint.Center => (rect.Size - size) / 2,
				_ => Vector2.Zero
			};

			child.Arrange(new(rect.Position.X + offset.X, rect.Position.Y + offset.Y, size.X, size.Y));
		}
	}
}
