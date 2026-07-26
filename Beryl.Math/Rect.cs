// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Runtime.CompilerServices;

namespace Beryl.Math;

/// <summary>
/// Represents a two-dimensional rectangle.
/// </summary>
public struct Rect
{
	/// <summary> The position of the <see cref="Rect"/>. </summary>
	public Vector2I Position { get; set; }

	/// <summary> The size/scale of the <see cref="Rect"/>. </summary>
	public Vector2I Size { get; set; }

	/// <summary> Creates a new instance of <see cref="Rect"/>. </summary>
	public Rect() { }

	/// <summary> Creates a new instance of <see cref="Rect"/> with the specified components. </summary>
	public Rect(int x, int y) => Position = new Vector2I(x, y);

	/// <summary> Creates a new instance of <see cref="Rect"/> with the specified components. </summary>
	public Rect(int x, int y, int width, int height)
	{
		Position = new Vector2I(x, y);
		Size = new Vector2I(width, height);
	}

	/// <summary> Checks if the specified coordinates are contained in the <see cref="Rect"/>. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(int x, int y) => x >= Position.X && x < Position.X + Size.X && y >= Position.Y && y < Position.Y + Size.Y;
}
