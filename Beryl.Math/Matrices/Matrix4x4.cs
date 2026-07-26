// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Math;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a four-dimensional matrix.
/// </summary>
public struct Matrix4x4 : IEquatable<Matrix4x4>
{
	#region Components
	public float M11 { get; set; }
	public float M12 { get; set; }
	public float M13 { get; set; }
	public float M14 { get; set; }

	public float M21 { get; set; }
	public float M22 { get; set; }
	public float M23 { get; set; }
	public float M24 { get; set; }

	public float M31 { get; set; }
	public float M32 { get; set; }
	public float M33 { get; set; }
	public float M34 { get; set; }

	public float M41 { get; set; }
	public float M42 { get; set; }
	public float M43 { get; set; }
	public float M44 { get; set; }
	#endregion

	/// <summary> Creates a new instance of <see cref="Matrix4x4"/> with the specified components. </summary>
	public Matrix4x4(
		float m11, float m12, float m13, float m14,
		float m21, float m22, float m23, float m24,
		float m31, float m32, float m33, float m34,
		float m41, float m42, float m43, float m44)
	{
		M11 = m11;
		M12 = m12;
		M13 = m13;
		M14 = m14;
		M21 = m21;
		M22 = m22;
		M23 = m23;
		M24 = m24;
		M31 = m31;
		M32 = m32;
		M33 = m33;
		M34 = m34;
		M41 = m41;
		M42 = m42;
		M43 = m43;
		M44 = m44;
	}

	/// <summary> An identity matrix, this is comparable to <see cref="Vector3.One"/> but for matrices. </summary>
	public static Matrix4x4 Identity => new(1, 0, 0, 0,
											0, 1, 0, 0,
											0, 0, 1, 0,
											0, 0, 0, 1);

	/// <summary> Returns whether this matrix is the identity matrix. </summary>
	public bool IsIdentity =>
		M11 == 1f && M22 == 1f && M33 == 1f && M44 == 1f &&
		M12 == 0f && M13 == 0f && M14 == 0f &&
		M21 == 0f && M23 == 0f && M24 == 0f &&
		M31 == 0f && M32 == 0f && M34 == 0f &&
		M41 == 0f && M42 == 0f && M43 == 0f;

	#region Factory Methods

	/// <summary> Creates a transformation matrix from the specified components. </summary>
	public static Matrix4x4 CreateTRS(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		Matrix4x4 translation = CreateTranslation(pos);
		Matrix4x4 rotation = CreateFromQuaternion(rot);
		Matrix4x4 scaling = CreateScale(scale);
		return scaling * rotation * translation;
	}

	/// <summary> Creates a translation matrix from a <see cref="Vector3"/>. </summary>
	public static Matrix4x4 CreateTranslation(Vector3 pos) => new(
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			pos.X, pos.Y, pos.Z, 1
		);

	/// <summary> Creates a translation matrix from the specified components. </summary>
	public static Matrix4x4 CreateTranslation(float x, float y, float z) => new(
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			x, y, z, 1
		);

	/// <summary> Creates a scaling matrix from a <see cref="Vector3"/>. </summary>
	public static Matrix4x4 CreateScale(Vector3 scale) => new(
			scale.X, 0, 0, 0,
			0, scale.Y, 0, 0,
			0, 0, scale.Z, 0,
			0, 0, 0, 1
		);

	/// <summary> Creates a scaling matrix from the specified components. </summary>
	public static Matrix4x4 CreateScale(float x, float y, float z) => new(
			x, 0, 0, 0,
			0, y, 0, 0,
			0, 0, z, 0,
			0, 0, 0, 1
		);

	/// <summary> Creates a rotation matrix around the X axis. </summary>
	public static Matrix4x4 CreateRotationX(float r)
	{
		float c = Math.Cos(r);
		float s = Math.Sin(r);

		return new Matrix4x4(
			1, 0, 0, 0,
			0, c, s, 0,
			0, -s, c, 0,
			0, 0, 0, 1
		);
	}

	/// <summary> Creates a rotation matrix around the Y axis. </summary>
	public static Matrix4x4 CreateRotationY(float r)
	{
		float c = Math.Cos(r);
		float s = Math.Sin(r);

		return new Matrix4x4(
			c, 0, -s, 0,
			0, 1, 0, 0,
			s, 0, c, 0,
			0, 0, 0, 1
		);
	}

	/// <summary> Creates a rotation matrix around the Z axis. </summary>
	public static Matrix4x4 CreateRotationZ(float r)
	{
		float c = Math.Cos(r);
		float s = Math.Sin(r);

		return new Matrix4x4(
			c, s, 0, 0,
			-s, c, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
		);
	}

	/// <summary> Creates a rotation matrix from Euler angles (XYZ order). </summary>
	public static Matrix4x4 CreateFromEuler(Vector3 euler)
	{
		Matrix4x4 rx = CreateRotationX(euler.X);
		Matrix4x4 ry = CreateRotationY(euler.Y);
		Matrix4x4 rz = CreateRotationZ(euler.Z);
		return rx * ry * rz;
	}

	/// <summary> Creates a rotation matrix from a <see cref="Quaternion"/>. </summary>
	public static Matrix4x4 CreateFromQuaternion(Quaternion q)
	{
		float xx = q.X * q.X;
		float yy = q.Y * q.Y;
		float zz = q.Z * q.Z;
		float xy = q.X * q.Y;
		float xz = q.X * q.Z;
		float yz = q.Y * q.Z;
		float wx = q.W * q.X;
		float wy = q.W * q.Y;
		float wz = q.W * q.Z;

		return new Matrix4x4(
			1 - 2 * (yy + zz), 2 * (xy + wz), 2 * (xz - wy), 0,
			2 * (xy - wz), 1 - 2 * (xx + zz), 2 * (yz + wx), 0,
			2 * (xz + wy), 2 * (yz - wx), 1 - 2 * (xx + yy), 0,
			0, 0, 0, 1
		);
	}

	/// <summary> Creates a right-handed LookAt view matrix. </summary>
	public static Matrix4x4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
	{
		var zAxis = Vector3.Normalize(target - eye); // Forward
		var xAxis = Vector3.Normalize(Vector3.Cross(up, zAxis)); // Right
		var yAxis = Vector3.Cross(zAxis, xAxis); // True up

		return new Matrix4x4(
			xAxis.X, yAxis.X, zAxis.X, 0,
			xAxis.Y, yAxis.Y, zAxis.Y, 0,
			xAxis.Z, yAxis.Z, zAxis.Z, 0,
			-Vector3.Dot(xAxis, eye),
			-Vector3.Dot(yAxis, eye),
			-Vector3.Dot(zAxis, eye),
			1
		);
	}

	#endregion

	#region Operations

	/// <summary> Returns the transposed version of this matrix. </summary>
	public Matrix4x4 Transpose() => new(
			M11, M21, M31, M41,
			M12, M22, M32, M42,
			M13, M23, M33, M43,
			M14, M24, M34, M44
		);

	/// <summary> Returns the determinant of this matrix. </summary>
	public float Determinant()
	{
		float a = M11 * (M22 * (M33 * M44 - M34 * M43) - M23 * (M32 * M44 - M34 * M42) + M24 * (M32 * M43 - M33 * M42));
		float b = M12 * (M21 * (M33 * M44 - M34 * M43) - M23 * (M31 * M44 - M34 * M41) + M24 * (M31 * M43 - M33 * M41));
		float c = M13 * (M21 * (M32 * M44 - M34 * M42) - M22 * (M31 * M44 - M34 * M41) + M24 * (M31 * M42 - M32 * M41));
		float d = M14 * (M21 * (M32 * M43 - M33 * M42) - M22 * (M31 * M43 - M33 * M41) + M23 * (M31 * M42 - M32 * M41));
		return a - b + c - d;
	}

	/// <summary> Returns the inverse of this matrix if invertible, otherwise the identity matrix. </summary>
	public Matrix4x4 Invert()
	{
		if (!System.Numerics.Matrix4x4.Invert((System.Numerics.Matrix4x4)this, out var inv))
			return Identity;
		return (Matrix4x4)inv;
	}

	/// <summary> Transforms a <see cref="Vector3"/> by this matrix. </summary>
	public Vector3 Transform(Vector3 v) => new(
			v.X * M11 + v.Y * M21 + v.Z * M31 + M41,
			v.X * M12 + v.Y * M22 + v.Z * M32 + M42,
			v.X * M13 + v.Y * M23 + v.Z * M33 + M43
		);

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Matrix4x4 other && Equals(other);

	/// <inheritdoc/>
	public bool Equals(Matrix4x4 other) => M11 == other.M11 && M12 == other.M12 && M13 == other.M13 && M14 == other.M14 &&
			M21 == other.M21 && M22 == other.M22 && M23 == other.M23 && M24 == other.M24 &&
			M31 == other.M31 && M32 == other.M32 && M33 == other.M33 && M34 == other.M34 &&
			M41 == other.M41 && M42 == other.M42 && M43 == other.M43 && M44 == other.M44;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = M11.GetHashCode();
			hashCode = (hashCode * 397) ^ M12.GetHashCode();
			hashCode = (hashCode * 397) ^ M13.GetHashCode();
			hashCode = (hashCode * 397) ^ M14.GetHashCode();
			hashCode = (hashCode * 397) ^ M21.GetHashCode();
			hashCode = (hashCode * 397) ^ M22.GetHashCode();
			hashCode = (hashCode * 397) ^ M23.GetHashCode();
			hashCode = (hashCode * 397) ^ M24.GetHashCode();
			hashCode = (hashCode * 397) ^ M31.GetHashCode();
			hashCode = (hashCode * 397) ^ M32.GetHashCode();
			hashCode = (hashCode * 397) ^ M33.GetHashCode();
			hashCode = (hashCode * 397) ^ M34.GetHashCode();
			hashCode = (hashCode * 397) ^ M41.GetHashCode();
			hashCode = (hashCode * 397) ^ M42.GetHashCode();
			hashCode = (hashCode * 397) ^ M43.GetHashCode();
			hashCode = (hashCode * 397) ^ M44.GetHashCode();
			return hashCode;
		}
	}

	#endregion

	#region Operators

	public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
	{
		return new Matrix4x4(
			a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31 + a.M14 * b.M41,
			a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32 + a.M14 * b.M42,
			a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33 + a.M14 * b.M43,
			a.M11 * b.M14 + a.M12 * b.M24 + a.M13 * b.M34 + a.M14 * b.M44,

			a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31 + a.M24 * b.M41,
			a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32 + a.M24 * b.M42,
			a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33 + a.M24 * b.M43,
			a.M21 * b.M14 + a.M22 * b.M24 + a.M23 * b.M34 + a.M24 * b.M44,

			a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31 + a.M34 * b.M41,
			a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32 + a.M34 * b.M42,
			a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33 + a.M34 * b.M43,
			a.M31 * b.M14 + a.M32 * b.M24 + a.M33 * b.M34 + a.M34 * b.M44,

			a.M41 * b.M11 + a.M42 * b.M21 + a.M43 * b.M31 + a.M44 * b.M41,
			a.M41 * b.M12 + a.M42 * b.M22 + a.M43 * b.M32 + a.M44 * b.M42,
			a.M41 * b.M13 + a.M42 * b.M23 + a.M43 * b.M33 + a.M44 * b.M43,
			a.M41 * b.M14 + a.M42 * b.M24 + a.M43 * b.M34 + a.M44 * b.M44
		);
	}

	#endregion

	#region Casts

	public static explicit operator System.Numerics.Matrix4x4(Matrix4x4 m)
		=> new(
			m.M11, m.M12, m.M13, m.M14,
			m.M21, m.M22, m.M23, m.M24,
			m.M31, m.M32, m.M33, m.M34,
			m.M41, m.M42, m.M43, m.M44
		);

	public static explicit operator Matrix4x4(System.Numerics.Matrix4x4 m)
		=> new(
			m.M11, m.M12, m.M13, m.M14,
			m.M21, m.M22, m.M23, m.M24,
			m.M31, m.M32, m.M33, m.M34,
			m.M41, m.M42, m.M43, m.M44
		);

	#endregion
}
