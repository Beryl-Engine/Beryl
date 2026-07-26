// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Math;

/// <summary>
/// Represents a four-dimensional point.
/// </summary>
public struct Vector4 : IEquatable<Vector4>
{
	/// <summary> The X component of the <see cref="Vector4"/>. </summary>
	public float X { get; set; }

	/// <summary> The Y component of the <see cref="Vector4"/>. </summary>
	public float Y { get; set; }

	/// <summary> The Z component of the <see cref="Vector4"/>. </summary>
	public float Z { get; set; }

	/// <summary> The W component of the <see cref="Vector4"/>. </summary>
	public float W { get; set; }

	/// <summary> The length of the <see cref="Vector4"/>. </summary>
	public float Length => Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

	/// <summary> Creates a new instance of <see cref="Vector4"/> with the specified components. </summary>
	public Vector4(float x, float y, float z, float w)
	{
		X = x;
		Y = y;
		Z = z;
		W = w;
	}

	/// <summary> A Vector with all components set to zero. </summary>
	public static Vector4 Zero => new(0, 0, 0, 0);

	/// <summary> A Vector with all components set to one. </summary>
	public static Vector4 One => new(1, 1, 1, 1);

	/// <summary> A Vector with X set to one. </summary>
	public static Vector4 UnitX => new(1, 0, 0, 0);

	/// <summary> A Vector with Y set to one. </summary>
	public static Vector4 UnitY => new(0, 1, 0, 0);

	/// <summary> A Vector with Z set to one. </summary>
	public static Vector4 UnitZ => new(0, 0, 1, 0);

	/// <summary> A Vector with W set to one. </summary>
	public static Vector4 UnitW => new(0, 0, 0, 1);

	/// <summary> Returns the dot product of the two vectors. </summary>
	public static float Dot(Vector4 v1, Vector4 v2) => v1.Dot(v2);

	/// <summary> Returns a normalized version of the <see cref="Vector4"/>. </summary>
	public static Vector4 Normalize(Vector4 v) => v.Normalize();

	/// <summary> Transforms a vector by a quaternion rotation. </summary>
	public static Vector4 Transform(Vector4 vector, Quaternion rotation)
	{
		// Extract the vector part of the quaternion
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

		// Preserve W
		return new Vector4(rx, ry, rz, vector.W);
	}

	/// <summary> Returns the dot product of the two vectors. </summary>
	public float Dot(Vector4 v) => X * v.X + Y * v.Y + Z * v.Z + W * v.W;

	/// <summary> Returns a normalized version of the <see cref="Vector4"/>. </summary>
	public Vector4 Normalize() => this / Length;

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		if (obj is Vector4 other)
			return Equals(other);

		return false;
	}

	/// <summary> Compares two <see cref="Vector4"/>s for equality. </summary>
	public bool Equals(Vector4 other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

	/// <inheritdoc/>
	override public string ToString() => $"({X}, {Y}, {Z}, {W})";

	#region Casts

	public static implicit operator System.Numerics.Vector4(Vector4 v) => new(v.X, v.Y, v.Z, v.W);
	public static implicit operator Vector4(System.Numerics.Vector4 v) => new(v.X, v.Y, v.Z, v.W);

	#endregion

	#region Operators
	public static Vector4 operator +(Vector4 a, Vector4 b)
		=> new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

	public static Vector4 operator -(Vector4 a, Vector4 b)
		=> new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

	public static Vector4 operator -(Vector4 v)
		=> new(-v.X, -v.Y, -v.Z, -v.W);

	public static Vector4 operator *(Vector4 a, float scalar)
		=> new(a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar);

	public static Vector4 operator *(float scalar, Vector4 a)
		=> new(a.X * scalar, a.Y * scalar, a.Z * scalar, a.W * scalar);

	public static Vector4 operator /(Vector4 a, float scalar)
		=> new(a.X / scalar, a.Y / scalar, a.Z / scalar, a.W / scalar);

	public static bool operator ==(Vector4 a, Vector4 b)
		=> a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

	public static bool operator !=(Vector4 a, Vector4 b)
		=> !(a == b);
	#endregion
}

