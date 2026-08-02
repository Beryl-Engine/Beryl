// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Math;

/// <summary>
/// Represents a two-dimensional point.
/// </summary>
public struct Vector2 : IEquatable<Vector2>
{
	/// <summary> The X component of the <see cref="Vector2"/>. </summary>
	public float X { get; set; }

	/// <summary> The Y component of the <see cref="Vector2"/>. </summary>
	public float Y { get; set; }

	/// <summary> The length of the <see cref="Vector2"/>. </summary>
	public float Length => Math.Sqrt(X * X + Y * Y);

	/// <summary> Creates a new instance of <see cref="Vector2"/> with the specified components. </summary>
	public Vector2(float x, float y)
	{
		X = x;
		Y = y;
	}

	/// <summary> A Vector with all components set to zero. </summary>
	public static Vector2 Zero => new(0, 0);

	/// <summary> A Vector with all components set to one. </summary>
	public static Vector2 One => new(1, 1);

	/// <summary> A Vector set to the world's up direction. </summary>
	public static Vector2 Up => new(0, 1);

	/// <summary> A Vector with X set to 1. </summary>
	public static Vector2 UnitX => new(1, 0);

	/// <summary> A Vector with Y set to 1. </summary>
	public static Vector2 UnitY => new(0, 1);

	/// <summary> Returns the dot product of the two vectors. </summary>
	public static float Dot(Vector2 v1, Vector2 v2) => v1.Dot(v2);

	/// <summary> Returns a normalized version of the <see cref="Vector2"/>. </summary>
	public static Vector2 Normalize(Vector2 v) => v.Normalize();

	/// <summary> Rotates a vector by the specified angle (in radians). </summary>
	public static Vector2 Rotate(Vector2 vector, float radians)
	{
		float cos = Math.Cos(radians);
		float sin = Math.Sin(radians);
		return new Vector2(
			vector.X * cos - vector.Y * sin,
			vector.X * sin + vector.Y * cos
		);
	}

	/// <summary> Returns the dot product of the two vectors. </summary>
	public float Dot(Vector2 v) => X * v.X + Y * v.Y;

	/// <summary> Returns a normalized version of the <see cref="Vector2"/>. </summary>
	public Vector2 Normalize() => this / Length;

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		if (obj is Vector2 other)
			return Equals(other);

		return false;
	}

	/// <summary> Compares two <see cref="Vector2"/>s for equality. </summary>
	public bool Equals(Vector2 other) => X == other.X && Y == other.Y;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(X, Y);

	/// <inheritdoc/>
	override public string ToString() => $"({X}, {Y})";

	#region Casts

	public static implicit operator System.Numerics.Vector2(Vector2 v) => new(v.X, v.Y);
	public static implicit operator Vector2(System.Numerics.Vector2 v) => new(v.X, v.Y);

	#endregion

	#region Operators
	public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);

	public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);

	public static Vector2 operator -(Vector2 v) => new(-v.X, -v.Y);

	public static Vector2 operator *(Vector2 a, float scalar) => new(a.X * scalar, a.Y * scalar);

	public static Vector2 operator *(float scalar, Vector2 a) => new(a.X * scalar, a.Y * scalar);

	public static Vector2 operator /(Vector2 a, float scalar) => new(a.X / scalar, a.Y / scalar);

	public static bool operator ==(Vector2 a, Vector2 b) => a.X == b.X && a.Y == b.Y;

	public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);
	#endregion
}
