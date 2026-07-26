// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;

using Silk.NET.Input;

namespace Beryl.Input.Devices;

public sealed class Mouse : IInputDevice
{
	/// <summary>
	/// Represents a mouse button.
	/// </summary>
	public enum Button
	{
		Left,
		Right,
		Middle
	}

	#region Silk.NET Mappings

	private static readonly Dictionary<Button, Silk.NET.Input.MouseButton> buttonMap = new()
	{
		{ Button.Left, Silk.NET.Input.MouseButton.Left },
		{ Button.Right, Silk.NET.Input.MouseButton.Right },
		{ Button.Middle, Silk.NET.Input.MouseButton.Middle }
	};

	#endregion

	/// <summary> The underlying Silk.NET <see cref="IMouse"/>. </summary>
	internal IMouse InternalMouse { get; }

	/// <summary> Checks if this mouse's cursor is currently locked. </summary>
	public bool CursorLocked
	{
		get => InternalMouse.Cursor.CursorMode == CursorMode.Raw && InternalMouse.Cursor.IsConfined; set => InternalMouse.Cursor.CursorMode = value ? CursorMode.Raw : CursorMode.Normal;
	}

	/// <summary> The change in mouse position since the last update. </summary>
	public Vector2 MouseDelta { get; private set; }
	private Vector2 lastMousePosition;

	public Mouse(IMouse mouse) => InternalMouse = mouse;

	/// <summary> Checks if the given <see cref="Button"/> is currently held down. </summary>
	public bool IsButtonDown(Button button) => InternalMouse.IsButtonPressed(buttonMap[button]);

	/// <inheritdoc/>
	public void Update()
	{
		var mousePos = InternalMouse.Position;
		MouseDelta = new Vector2(mousePos.X - lastMousePosition.X, mousePos.Y - lastMousePosition.Y);
		lastMousePosition = mousePos;
	}
}
