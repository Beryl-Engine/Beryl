// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Silk.NET.Input;

namespace Beryl.Input.Devices;

public sealed class Keyboard : IInputDevice
{
	/// <summary>
	/// Represents a key on a Keyboard.
	/// </summary>
	public enum Key
	{
		A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
		Space,
		Enter,
		Backspace,
		Escape
	}

	#region Silk.NET Mappings

	private static readonly Dictionary<Key, Silk.NET.Input.Key> keyMap = new()
	{
		{ Key.A, Silk.NET.Input.Key.A },
		{ Key.B, Silk.NET.Input.Key.B },
		{ Key.C, Silk.NET.Input.Key.C },
		{ Key.D, Silk.NET.Input.Key.D },
		{ Key.E, Silk.NET.Input.Key.E },
		{ Key.F, Silk.NET.Input.Key.F },
		{ Key.G, Silk.NET.Input.Key.G },
		{ Key.H, Silk.NET.Input.Key.H },
		{ Key.I, Silk.NET.Input.Key.I },
		{ Key.J, Silk.NET.Input.Key.J },
		{ Key.K, Silk.NET.Input.Key.K },
		{ Key.L, Silk.NET.Input.Key.L },
		{ Key.M, Silk.NET.Input.Key.M },
		{ Key.N, Silk.NET.Input.Key.N },
		{ Key.O, Silk.NET.Input.Key.O },
		{ Key.P, Silk.NET.Input.Key.P },
		{ Key.Q, Silk.NET.Input.Key.Q },
		{ Key.R, Silk.NET.Input.Key.R },
		{ Key.S, Silk.NET.Input.Key.S },
		{ Key.T, Silk.NET.Input.Key.T },
		{ Key.U, Silk.NET.Input.Key.U },
		{ Key.V, Silk.NET.Input.Key.V },
		{ Key.W, Silk.NET.Input.Key.W },
		{ Key.X, Silk.NET.Input.Key.X },
		{ Key.Y, Silk.NET.Input.Key.Y },
		{ Key.Z, Silk.NET.Input.Key.Z },
		{ Key.Space, Silk.NET.Input.Key.Space },
		{ Key.Enter, Silk.NET.Input.Key.Enter },
		{ Key.Backspace, Silk.NET.Input.Key.Backspace },
		{ Key.Escape, Silk.NET.Input.Key.Escape }
	};

	#endregion

	private readonly Dictionary<Key, bool> previousState = new();
	private readonly Dictionary<Key, bool> currentState = new();

	/// <summary> The underlying Silk.NET <see cref="IKeyboard"/>. </summary>
	internal IKeyboard InternalKeyboard { get; }

	public Keyboard(IKeyboard keyboard)
	{
		InternalKeyboard = keyboard;

		foreach (var key in keyMap.Keys)
		{
			previousState[key] = false;
			currentState[key] = false;
		}
	}

	/// <summary> Checks if the given <see cref="Key"/> is currently held down. </summary>
	public bool IsKeyDown(Key key) => InternalKeyboard.IsKeyPressed(keyMap[key]);

	/// <summary> Checks if the given <see cref="Key"/> was pressed this frame. </summary>
	public bool IsKeyPressed(Key key) => currentState[key] && !previousState[key];

	/// <summary> Checks if the given <see cref="Key"/> was released this frame. </summary>
	public bool IsKeyReleased(Key key) => !currentState[key] && previousState[key];

	/// <inheritdoc/>
	public void Update()
	{
		foreach (var key in keyMap.Keys)
		{
			previousState[key] = currentState[key];
			currentState[key] = InternalKeyboard.IsKeyPressed(keyMap[key]);
		}
	}
}
