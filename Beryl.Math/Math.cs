// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Beryl.Math;

/// <summary>
/// Provides common math utility.
/// </summary>
/// <remarks>
/// This is a generic math implementation.
/// </remarks>
public static class Math
{
	/// <summary> The ratio of a circle's circumference to its diameter. </summary>
	public static float PI => 3.14159265f;

	/// <summary> Returns the smaller of two values. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Min<T>(T a, T b) where T : IComparisonOperators<T, T, bool> => a < b ? a : b;

	/// <summary> Returns the greater of two values. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Max<T>(T a, T b) where T : IComparisonOperators<T, T, bool> => a > b ? a : b;

	/// <summary> Returns the absolute value of the specified value. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Abs<T>(T value) where T : INumber<T> => value < T.Zero ? -value : value;

	/// <summary> Restricts a value to the inclusive range defined by the minimum and maximum bounds. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Clamp<T>(T value, T min, T max) where T : IComparisonOperators<T, T, bool> => Min(Max(value, min), max);

	/// <summary> Returns the sine of the specified angle in radians. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sin<T>(T angle) where T : ITrigonometricFunctions<T> => T.Sin(angle);

	/// <summary> Returns the cosine of the specified angle in radians. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Cos<T>(T angle) where T : ITrigonometricFunctions<T> => T.Cos(angle);

	/// <summary> Returns the tangent of the specified angle in radians. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Tan<T>(T angle) where T : ITrigonometricFunctions<T> => T.Tan(angle);

	/// <summary> Returns the inverse tangent of the specified value in radians. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ATan<T>(T value) where T : ITrigonometricFunctions<T> => T.Atan(value);

	/// <summary> Returns the angle in radians between the positive x-axis and the specified point. </summary>
	/// <remarks> The result is determined using the signs of both inputs to determine the correct quadrant. </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ATan2<T>(T y, T x) where T : IFloatingPointIeee754<T> => T.Atan2(y, x);

	/// <summary> Returns the square root of the specified value. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sqrt<T>(T value) where T : IRootFunctions<T> => T.Sqrt(value);

	/// <summary> Returns the integer square root of the specified value. </summary>
	/// <remarks> The result is rounded down to the nearest integer. </remarks>
	public static T ISqrt<T>(T value) where T : IBinaryInteger<T>
	{
		ArgumentOutOfRangeException.ThrowIfNegative(value);

		if (value < T.CreateChecked(2))
			return value;

		T x = value;
		T y = (x + T.One) >> 1;

		while (y < x)
		{
			x = y;
			y = (x + value / x) >> 1;
		}

		return x;
	}

	/// <summary> Returns the inverse sine of the specified value in radians. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Asin<T>(T value) where T : ITrigonometricFunctions<T> => T.Asin(value);

	/// <summary> Returns a value with the magnitude of one value and the sign of another. </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T CopySign<T>(T value, T sign) where T : INumber<T> => sign < T.Zero ? -Abs(value) : Abs(value);
}
