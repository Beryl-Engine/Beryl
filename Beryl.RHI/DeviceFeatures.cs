// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.RHI;

/// <summary>
/// Flags that define different features supported by a <see cref="IGraphicsDevice"/> backend.
/// </summary>
[Flags]
public enum DeviceFeatures : ushort
{
	None = 0,

	ClipSpaceYInverted = 1 << 0
}
