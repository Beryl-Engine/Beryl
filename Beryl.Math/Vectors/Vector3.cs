// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Math;

/// <summary>
/// Represents a three-dimensional point.
/// </summary>
public struct Vector3 : IEquatable<Vector3>
{
	/// <summary> The X component of the <see cref="Vector3"/>. </summary>
	public float X { get; set; }

	/// <summary> The Y component of the <see cref="Vector3"/>. </summary>
	public float Y { get; set; }

	/// <summary> The Z component of the <see cref="Vector3"/>. </summary>
	public float Z { get; set; }

	/// <summary> The length of the <see cref="Vector3"/>. </summary>
	public float Length => Math.Sqrt(X * X + Y * Y + Z * Z);

	/// <summary> Creates a new instance of <see cref="Vector3"/> with the specified components. </summary>
	public Vector3(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	/// <summary> A Vector with all components set to zero. </summary>
	public static Vector3 Zero => new(0, 0, 0);

	/// <summary> A Vector with all components set to one. </summary>
	public static Vector3 One => new(1, 1, 1);

	/// <summary> A Vector set to the world's up direction. </summary>
	public static Vector3 Up => new(0, 1, 0);

	/// <summary> A Vector set to the world's right direction. </summary>
	public static Vector3 Right => new(1, 0, 0);

	/// <summary> A Vector set to the world's forward direction. </summary>
	public static Vector3 Forward => new(0, 0, 1);

	/// <summary> A Vector with X set to one. </summary>
	public static Vector3 UnitX => new(1, 0, 0);

	/// <summary> A Vector with Y set to one. </summary>
	public static Vector3 UnitY => new(0, 1, 0);

	/// <summary> A Vector with Z set to one. </summary>
	public static Vector3 UnitZ => new(0, 0, 1);

	/// <summary> Returns the dot product of the two vectors. </summary>
	public static float Dot(Vector3 v1, Vector3 v2) => v1.Dot(v2);

	/// <summary> Returns a normalized version of the <see cref="Vector3"/>. </summary>
	public static Vector3 Normalize(Vector3 v) => v.Normalize();

	/// <summary> Returns the cross product of the two vectors. </summary>
	public static Vector3 Cross(Vector3 v1, Vector3 v2) => v1.Cross(v2);

	/// <summary> Transforms a vector by a quaternion rotation. </summary>
	public static Vector3 Transform(Vector3 vector, Quaternion rotation)
	{
		// Extract quaternion components
		float qx = rotation.X;
		float qy = rotation.Y;
		float qz = rotation.Z;
		float qw = rotation.W;

		// t = 2 * cross(q.xyz, v)
		float tx = 2f * (qy * vector.Z - qz * vector.Y);
		float ty = 2f * (qz * vector.X - qx * vector.Z);
		float tz = 2f * (qx * vector.Y - qy * vector.X);

		// v' = v + qw * t + cross(q.xyz, t)
		float rx = vector.X + qw * tx + (qy * tz - qz * ty);
		float ry = vector.Y + qw * ty + (qz * tx - qx * tz);
		float rz = vector.Z + qw * tz + (qx * ty - qy * tx);

		return new Vector3(rx, ry, rz);
	}

	/// <summary> Returns the dot product of the two vectors. </summary>
	public float Dot(Vector3 v) => X * v.X + Y * v.Y + Z * v.Z;

	/// <summary> Returns a normalized version of the <see cref="Vector3"/>. </summary>
	public Vector3 Normalize() => this / Length;

	/// <summary> Returns the cross product of the two vectors. </summary>
	public Vector3 Cross(Vector3 v) => new(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		if (obj is Vector3 other)
			return Equals(other);

		return false;
	}

	/// <summary> Compares two <see cref="Vector3"/>s for equality. </summary>
	public bool Equals(Vector3 other) => X == other.X && Y == other.Y && Z == other.Z;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(X, Y, Z);

	/// <inheritdoc/>
	override public string ToString() => $"({X}, {Y}, {Z})";

	#region Casts

	public static implicit operator System.Numerics.Vector3(Vector3 v) => new(v.X, v.Y, v.Z);
	public static implicit operator Vector3(System.Numerics.Vector3 v) => new(v.X, v.Y, v.Z);

	#endregion

	#region Operators
	public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

	public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

	public static Vector3 operator -(Vector3 v) => new(-v.X, -v.Y, -v.Z);

	public static Vector3 operator *(Vector3 a, float scalar) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

	public static Vector3 operator *(float scalar, Vector3 a) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

	public static Vector3 operator /(Vector3 a, float scalar) => new(a.X / scalar, a.Y / scalar, a.Z / scalar);

	public static bool operator ==(Vector3 a, Vector3 b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

	public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);
	#endregion
}
