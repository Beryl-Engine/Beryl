// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Input.Devices;

namespace Beryl.VirtualReality;

/// <summary>
/// Base class for all Virtual Reality motion controllers.
/// </summary>
public abstract class VirtualRealityController : IInputDevice
{
	/// <summary>
	/// Represents a Virtual Reality controller button.
	/// </summary>
	public enum Button
	{
		ActionUp,
		ActionDown,

		/// <summary> Controllers system button. (i.e., the menu button) </summary>
		System
	}

	public enum HandType
	{
		Left,
		Right
	}

	/// <summary> Which hand this controller is attached to. </summary>
	public HandType Handness { get; protected set; }

	/// <summary> How much the trigger is pressed. </summary>
	public float TriggerPressedAmount { get; protected set; }

	/// <summary> How much the grip button is pressed. </summary>
	public float GripPressedAmount { get; protected set; }

	/// <summary> Checks if the given <see cref="Button"/> is currently held down. </summary>
	public abstract bool IsButtonPressed(Button button);

	/// <inheritdoc/>
	public abstract void Update();
}
