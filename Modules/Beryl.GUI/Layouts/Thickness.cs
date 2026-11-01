// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;

namespace Beryl.GUI.Layouts;

public readonly record struct Thickness(float Left, float Top, float Right, float Bottom)
{
	public Thickness(float uniform) : this(uniform, uniform, uniform, uniform) { }
	public Thickness(float horizontal, float vertical) : this(horizontal, vertical, horizontal, vertical) { }

	internal Vector2 Size => new(Left + Right, Top + Bottom);
	internal Rect Deflate(Rect rect) => new(rect.Position.X + Left, rect.Position.Y + Top, Math.Math.Max(0, rect.Size.X - Left - Right), Math.Math.Max(0, rect.Size.Y - Top - Bottom));
}
