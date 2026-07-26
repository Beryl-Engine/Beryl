// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Input.Devices;

/// <summary>
/// Base interface for all Input Devices. (i.e., Controllers, Keyboards, etc.)
/// </summary>
public interface IInputDevice
{
	/// <summary> Runs once per <see cref="InputModule"/> update. </summary>
	void Update();
}
