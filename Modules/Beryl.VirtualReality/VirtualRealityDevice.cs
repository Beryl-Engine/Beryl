// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.VirtualReality;

/// <summary>
/// Base class for all Virtual Reality devices.
/// </summary>
/// <remarks>
/// "Virtual Reality Device" is defined as any device that provides 3D input and visualization for a world to the user.
/// Each implementation of <see cref="VirtualRealityDevice"/> is required to add their own <see cref="VirtualRealityController"/>s to the Input system.
/// </remarks>
public abstract class VirtualRealityDevice
{
	public string Name { get; set; } = "Unknown VR Device";

	public abstract void Initialize();
	public virtual void Update() { }
}
