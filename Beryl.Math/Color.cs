// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Math;

/// <summary>
/// Represents a color.
/// </summary>
public struct Color : IEquatable<Color>
{
	/// <summary> The red component of the color. </summary>
	public float R { get; set; }

	/// <summary> The green component of the color. </summary>
	public float G { get; set; }

	/// <summary> The blue component of the color. </summary>
	public float B { get; set; }

	/// <summary> The alpha (transparency) component of the color. </summary>
	public float A { get; set; }

	/// <summary> Initializes a new instance of <see cref="Color"/> with the specified components. </summary>
	public Color(float r, float g, float b, float a) => (R, G, B, A) = (r, g, b, a);
	#region Predefined Colors
	public static Color Transparent => new(0, 0, 0, 0);
	public static Color White => new(1, 1, 1, 1);
	public static Color Black => new(0, 0, 0, 1);
	public static Color Red => new(1, 0, 0, 1);
	public static Color Green => new(0, 1, 0, 1);
	public static Color Blue => new(0, 0, 1, 1);
	#endregion

	public override bool Equals(object? obj)
	{
		if (obj is Color other)
			return Equals(other);

		return false;
	}

	public bool Equals(Color other) => R == other.R && G == other.G && B == other.B && A == other.A;

	public override int GetHashCode() => HashCode.Combine(R, G, B, A);

	/// <summary> Creates a <see cref="Color"/> from 0-255 values. </summary>
	public static Color From255(int r, int g, int b, int a) => new(r / 255f, g / 255f, b / 255f, a / 255f);

	public static explicit operator Vector4(Color color) => new(color.R, color.G, color.B, color.A);

	public static Color operator *(Color color, float scalar) => new(color.R * scalar, color.G * scalar, color.B * scalar, color.A * scalar);

	public static Color operator /(Color color, float scalar) => new(color.R / scalar, color.G / scalar, color.B / scalar, color.A / scalar);

	public static bool operator ==(Color a, Color b) => a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;

	public static bool operator !=(Color a, Color b) => !(a == b);

}
