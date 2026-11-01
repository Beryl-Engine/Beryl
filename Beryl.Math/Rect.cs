// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Runtime.CompilerServices;

namespace Beryl.Math;

/// <summary>
/// Represents a two-dimensional rectangle with floating-point coordinates.
/// </summary>
public struct Rect
{
	/// <summary> The position of the <see cref="Rect"/>. </summary>
	public Vector2 Position { get; set; }

	/// <summary> The size of the <see cref="Rect"/>. </summary>
	public Vector2 Size { get; set; }

	/// <summary> Creates a new instance of <see cref="Rect"/>. </summary>
	public Rect() { }

	/// <summary> Creates a new instance of <see cref="Rect"/> with the specified components. </summary>
	public Rect(float x, float y) => Position = new(x, y);

	/// <summary> Creates a new instance of <see cref="Rect"/> with the specified components. </summary>
	public Rect(float x, float y, float width, float height)
	{
		Position = new(x, y);
		Size = new(width, height);
	}

	/// <summary> Checks if the specified coordinates are contained in the <see cref="Rect"/>. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(float x, float y) => x >= Position.X && x < Position.X + Size.X && y >= Position.Y && y < Position.Y + Size.Y;

	public static implicit operator Rect(RectI rect) => new(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y);
}
