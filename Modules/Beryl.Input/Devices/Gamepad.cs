// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Silk.NET.Input;

namespace Beryl.Input.Devices;

public sealed class Gamepad : IInputDevice
{
	/// <summary>
	/// Represents a Gamepad button.
	/// </summary>
	public enum Button
	{
		LeftShoulder,
		RightShoulder,
		DPadUp,
		DPadDown,
		DPadLeft,
		DPadRight,
		ActionDown,
		ActionRight,
		ActionLeft,
		ActionUp
	}

	/// <summary>
	/// Represents a Gamepad trigger.
	/// </summary>
	public enum Trigger
	{
		Left,
		Right
	}

	#region Silk.NET Mappings

	private static readonly Dictionary<Button, ButtonName> buttonMap = new()
	{
		{ Button.LeftShoulder, ButtonName.LeftBumper },
		{ Button.RightShoulder, ButtonName.RightBumper },
		{ Button.DPadUp, ButtonName.DPadUp },
		{ Button.DPadDown, ButtonName.DPadDown },
		{ Button.DPadLeft, ButtonName.DPadLeft },
		{ Button.DPadRight, ButtonName.DPadRight },
		{ Button.ActionDown, ButtonName.A },
		{ Button.ActionRight, ButtonName.B },
		{ Button.ActionLeft, ButtonName.X },
		{ Button.ActionUp, ButtonName.Y }
	};

	#endregion

	/// <summary> The underlying Silk.NET <see cref="IGamepad"/>. </summary>
	internal IGamepad InternalGamepad { get; }

	public Gamepad(IGamepad controller) => InternalGamepad = controller;

	/// <summary> Checks if the given <see cref="Button"/> is currently held down. </summary>
	public bool IsButtonPressed(Button button)
	{
		var silkButtonName = buttonMap[button];

		foreach (var silkButton in InternalGamepad.Buttons)
		{
			if (silkButton.Name == silkButtonName)
				return silkButton.Pressed;
		}

		return false;
	}

	/// <summary> Checks if the given <see cref="Button"/> is currently held down. </summary>
	public float GetTrigger(Trigger trigger)
	{
		foreach (var availableTrigger in InternalGamepad.Triggers)
		{
			if (trigger == Trigger.Right && availableTrigger.Index == 1)
				return availableTrigger.Position;

			if (trigger == Trigger.Left && availableTrigger.Index == 0)
				return availableTrigger.Position;
		}

		return 0f;
	}

	/// <inheritdoc/>
	public void Update() { }
}
