// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Math;

/// <summary>
/// Represents a two-dimensional integer point.
/// </summary>
public struct Vector2I : IEquatable<Vector2I>
{
	/// <summary> The X component of the <see cref="Vector2I"/>. </summary>
	public int X { get; set; }

	/// <summary> The Y component of the <see cref="Vector2I"/>. </summary>
	public int Y { get; set; }

	/// <summary> The length of the <see cref="Vector2I"/> as a float. </summary>
	public float Length => Math.ISqrt(X * X + Y * Y);

	/// <summary> Creates a new instance of <see cref="Vector2I"/> with the specified components. </summary>
	public Vector2I(int x, int y)
	{
		X = x;
		Y = y;
	}

	/// <summary> A Vector with all components set to zero. </summary>
	public static Vector2I Zero => new(0, 0);

	/// <summary> A Vector with all components set to one. </summary>
	public static Vector2I One => new(1, 1);

	/// <summary> A Vector set to the world's up direction. </summary>
	public static Vector2I Up => new(0, 1);

	/// <summary> A Vector with X set to 1. </summary>
	public static Vector2I UnitX => new(1, 0);

	/// <summary> A Vector with Y set to 1. </summary>
	public static Vector2I UnitY => new(0, 1);

	/// <summary> Returns the dot product of the two vectors. </summary>
	public static int Dot(Vector2I v1, Vector2I v2) => v1.Dot(v2);

	/// <summary> Returns the dot product of the two vectors. </summary>
	public int Dot(Vector2I v) => X * v.X + Y * v.Y;

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		if (obj is Vector2I other)
			return Equals(other);

		return false;
	}

	/// <summary> Compares two <see cref="Vector2I"/>s for equality. </summary>
	public bool Equals(Vector2I other) => X == other.X && Y == other.Y;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(X, Y);

	/// <inheritdoc/>
	public override string ToString() => $"({X}, {Y})";

	#region Operators
	public static Vector2I operator +(Vector2I a, Vector2I b)
		=> new(a.X + b.X, a.Y + b.Y);

	public static Vector2I operator -(Vector2I a, Vector2I b)
		=> new(a.X - b.X, a.Y - b.Y);

	public static Vector2I operator -(Vector2I v)
		=> new(-v.X, -v.Y);

	public static Vector2I operator *(Vector2I a, int scalar)
		=> new(a.X * scalar, a.Y * scalar);

	public static Vector2I operator *(int scalar, Vector2I a)
		=> new(a.X * scalar, a.Y * scalar);

	public static bool operator ==(Vector2I a, Vector2I b)
		=> a.X == b.X && a.Y == b.Y;

	public static bool operator !=(Vector2I a, Vector2I b)
		=> !(a == b);
	#endregion
}
