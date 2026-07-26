// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Common;

/// <summary>
/// Management of the applications time tracking.
/// </summary>
public static class Time
{
	/// <summary> Fixed delta used for physics simulations. </summary>
	public static float FixedDelta { get; } = 1f / 55f;

	/// <summary> The time elapsed since the last frame in seconds. </summary>
	public static float Delta { get; internal set; }

	/// <summary> The precise time elapsed since the last frame in seconds. </summary>
	/// <remarks> This value is more precise than <see cref="Delta"/> and is useful for high-precision simulations. For other purposes, use <see cref="Delta"/>. </remarks>
	public static double PreciseDelta { get; internal set; }
}
