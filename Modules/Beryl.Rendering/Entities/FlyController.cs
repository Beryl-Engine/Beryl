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
	public float Sensitivity { get; set; } = 0.0025f;
	public float Speed { get; set; } = 5f;

	[ImplicitComponent] 
	protected Camera Camera { get; set; } = null!;

	private float pitch;
	private float yaw;

	private bool cursorLocked = false;

	/// <inheritdoc/>
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
			Transform.Position += Transform.Forward * Speed * Time.Delta;
		if (Input.PrimaryKeyboard?.IsKeyDown(Keyboard.Key.S) ?? false)
			Transform.Position -= Transform.Forward * Speed * Time.Delta;
		if (Input.PrimaryKeyboard?.IsKeyDown(Keyboard.Key.A) ?? false)
			Transform.Position -= Transform.Right * Speed * Time.Delta;
		if (Input.PrimaryKeyboard?.IsKeyDown(Keyboard.Key.D) ?? false)
			Transform.Position += Transform.Right * Speed * Time.Delta;

		if (!cursorLocked)
			return;

		// Mouse look
		Vector2 delta = Input.PrimaryMouse?.MouseDelta ?? Vector2.Zero;

		yaw += delta.X * Sensitivity;
		pitch += delta.Y * Sensitivity;

		pitch = Math.Math.Clamp(pitch, -1.55f, 1.55f);

		var yawRot = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);

		var right = Vector3.Transform(Vector3.Right, yawRot);
		var pitchRot = Quaternion.CreateFromAxisAngle(right, pitch);

		Transform.Rotation = pitchRot * yawRot;
	}
}
