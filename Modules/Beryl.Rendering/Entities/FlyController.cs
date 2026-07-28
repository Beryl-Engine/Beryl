// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.Common.Utility;
using Beryl.Input;
using Beryl.Input.Devices;
using Beryl.Math;
using Beryl.Physics;
using Beryl.Rendering.Components;
using Beryl.Scenes.Entities;

namespace Beryl.Rendering.Entities;

[Entity("fly_controller")]
public class FlyController : Entity
{
	[ImplicitComponentAttribute] public Camera Camera { get; set; } = null!;

	private readonly float mouseSensitivity = 0.0025f;
	private readonly float moveSpeed = 8.0f;

	private float pitch;
	private float yaw;

	bool cursorLocked = false;

	public override void Update()
	{
		base.Update();

		var Input = ModuleManager.GetModule<InputModule>();
		if (Input == null)
			return;

		if (Input.PrimaryKeyboard?.IsKeyPressed(Keyboard.Key.Escape) ?? false)
		{
			cursorLocked = !cursorLocked;
			Input.PrimaryMouse?.CursorLocked = cursorLocked;
		}

		if (Input.PrimaryKeyboard?.IsKeyPressed(Keyboard.Key.E) ?? false)
		{
			if (ModuleManager.GetModule<PhysicsModule>()?.World.TryRaycast(Transform.Position, Transform.Forward, 100f, out var hit) ?? false)
				BerylConsole.Log(hit.Body.Owner?.Name);
		}

		// Movement
		if (Input.PrimaryKeyboard?.IsKeyDown(Keyboard.Key.W) ?? false)
			Transform.Position += Transform.Forward * moveSpeed * Time.Delta;
		if (Input.PrimaryKeyboard?.IsKeyDown(Keyboard.Key.S) ?? false)
			Transform.Position -= Transform.Forward * moveSpeed * Time.Delta;
		if (Input.PrimaryKeyboard?.IsKeyDown(Keyboard.Key.A) ?? false)
			Transform.Position -= Transform.Right * moveSpeed * Time.Delta;
		if (Input.PrimaryKeyboard?.IsKeyDown(Keyboard.Key.D) ?? false)
			Transform.Position += Transform.Right * moveSpeed * Time.Delta;

		if (!cursorLocked)
			return;

		// Mouse look
		Vector2 delta = Input.PrimaryMouse?.MouseDelta ?? Vector2.Zero;

		yaw += delta.X * mouseSensitivity;
		pitch += delta.Y * mouseSensitivity;

		pitch = Math.Math.Clamp(pitch, -1.55f, 1.55f);

		var yawRot = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);

		var right = Vector3.Transform(Vector3.Right, yawRot);
		var pitchRot = Quaternion.CreateFromAxisAngle(right, pitch);

		Transform.Rotation = pitchRot * yawRot;
	}
}
