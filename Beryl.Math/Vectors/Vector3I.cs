// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Math;

/// <summary>
/// Represents a three-dimensional integer point.
/// </summary>
public struct Vector3I : IEquatable<Vector3I>
{
	/// <summary> The X component of the <see cref="Vector3I"/>. </summary>
	public int X { get; set; }

	/// <summary> The Y component of the <see cref="Vector3I"/>. </summary>
	public int Y { get; set; }

	/// <summary> The Z component of the <see cref="Vector3I"/>. </summary>
	public int Z { get; set; }

	/// <summary> The length of the <see cref="Vector3I"/> as a float. </summary>
	public float Length => Math.ISqrt(X * X + Y * Y + Z * Z);

	/// <summary> Creates a new instance of <see cref="Vector3I"/> with the specified components. </summary>
	public Vector3I(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	/// <summary> A Vector with all components set to zero. </summary>
	public static Vector3I Zero => new(0, 0, 0);

	/// <summary> A Vector with all components set to one. </summary>
	public static Vector3I One => new(1, 1, 1);

	/// <summary> A Vector set to the world's up direction. </summary>
	public static Vector3I Up => new(0, 1, 0);

	/// <summary> A Vector with X set to 1. </summary>
	public static Vector3I UnitX => new(1, 0, 0);

	/// <summary> A Vector with Y set to 1. </summary>
	public static Vector3I UnitY => new(0, 1, 0);

	/// <summary> A Vector with Z set to 1. </summary>
	public static Vector3I UnitZ => new(0, 0, 1);

	/// <summary> Returns the dot product of the two vectors. </summary>
	public static int Dot(Vector3I v1, Vector3I v2) => v1.Dot(v2);

	/// <summary> Returns the dot product of the two vectors. </summary>
	public int Dot(Vector3I v) => X * v.X + Y * v.Y + Z * v.Z;

	/// <summary> Returns the cross product of the two vectors. </summary>
	public static Vector3I Cross(Vector3I v1, Vector3I v2) => v1.Cross(v2);

	/// <summary> Returns the cross product of the two vectors. </summary>
	public Vector3I Cross(Vector3I v) => new(
		Y * v.Z - Z * v.Y,
		Z * v.X - X * v.Z,
		X * v.Y - Y * v.X);

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		if (obj is Vector3I other)
			return Equals(other);

		return false;
	}

	/// <summary> Compares two <see cref="Vector3I"/>s for equality. </summary>
	public bool Equals(Vector3I other) => X == other.X && Y == other.Y && Z == other.Z;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(X, Y, Z);

	/// <inheritdoc/>
	public override string ToString() => $"({X}, {Y}, {Z})";

	#region Operators
	public static Vector3I operator +(Vector3I a, Vector3I b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

	public static Vector3I operator -(Vector3I a, Vector3I b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

	public static Vector3I operator -(Vector3I v) => new(-v.X, -v.Y, -v.Z);

	public static Vector3I operator *(Vector3I a, int scalar) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

	public static Vector3I operator *(int scalar, Vector3I a) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

	public static bool operator ==(Vector3I a, Vector3I b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

	public static bool operator !=(Vector3I a, Vector3I b) => !(a == b);
	#endregion
}
