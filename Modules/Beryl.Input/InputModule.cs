// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.Input.Devices;

using Silk.NET.Input;

namespace Beryl.Input;

/// <summary>
/// Module responsible for handling user input.
/// </summary>
public class InputModule : BaseModule
{
	/// <summary> All currently connected input devices. </summary>
	public IReadOnlyList<Devices.IInputDevice> Devices => devices;

	/// <summary> The primary (first connected) <see cref="Keyboard"/>. </summary>
	public Keyboard? PrimaryKeyboard { get; private set; }

	/// <summary> The primary (first connected) <see cref="Mouse"/>. </summary>
	public Mouse? PrimaryMouse { get; private set; }

	/// <summary> The primary (first connected) <see cref="Gamepad"/>. </summary>
	public Gamepad? PrimaryGamepad { get; private set; }

	private readonly List<Devices.IInputDevice> devices = new();

	private IInputContext input = null!;

	public void AddDevice(Devices.IInputDevice device) => devices.Add(device);

	/// <inheritdoc/>
	public override void Initialize()
	{
		Application.OnUpdate += Update;

		input = Application.Window.InternalWindow.CreateInput();

		foreach (var keyboard in input.Keyboards)
			devices.Add(new Keyboard(keyboard));

		foreach (var mouse in input.Mice)
			devices.Add(new Mouse(mouse));

		foreach (var gamepad in input.Gamepads)
			devices.Add(new Gamepad(gamepad));

		PrimaryKeyboard = devices.OfType<Keyboard>().FirstOrDefault();
		PrimaryMouse = devices.OfType<Mouse>().FirstOrDefault();
		PrimaryGamepad = devices.OfType<Gamepad>().FirstOrDefault();
	}

	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();

		Application.OnUpdate -= Update;

		input.Dispose();
	}

	public void Update()
	{
		foreach (var device in devices)
			device.Update();
	}
}
