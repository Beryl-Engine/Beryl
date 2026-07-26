// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.Math;
using Beryl.Scenes.Components;
using Beryl.Scenes.Entities;

namespace Beryl.Physics.Components;

/// <summary>
/// Represents a physics rigid body that enables simulation of physical interactions for an <see cref="Entity"/>.
/// </summary>
public class RigidBody : Component
{
	private IPhysicsBody? body;

	/// <inheritdoc/>
	public bool IsStatic
	{
		get => body?.IsStatic ?? false;
		set => body?.IsStatic = value;
	}

	/// <inheritdoc/>
	public bool AffectedByGravity
	{
		get => body?.AffectedByGravity ?? false;
		set => body?.AffectedByGravity = value;
	}

	/// <inheritdoc/>
	public float Mass
	{
		get => body?.Mass ?? 0f;
		set => body?.Mass = value;
	}

	/// <inheritdoc/>
	public float Friction
	{
		get => body?.Friction ?? 0f;
		set => body?.Friction = value;
	}

	/// <inheritdoc/>
	public float Restitution
	{
		get => body?.Restitution ?? 0f;
		set => body?.Restitution = value;
	}

	/// <inheritdoc/>
	public Vector3 Position
	{
		get => body?.Position ?? Vector3.Zero;
		set => body?.Position = value;
	}

	/// <inheritdoc/>
	public Quaternion Orientation
	{
		get => body?.Orientation ?? new Quaternion(0, 0, 0, 1);
		set => body?.Orientation = value;
	}

	/// <inheritdoc/>
	public Vector3 Velocity
	{
		get => body?.Velocity ?? Vector3.Zero;
		set => body?.Velocity = value;
	}

	/// <inheritdoc/>
	public Vector3 AngularVelocity
	{
		get => body?.AngularVelocity ?? Vector3.Zero;
		set => body?.AngularVelocity = value;
	}

	/// <inheritdoc/>
	public Vector2 Damping
	{
		get => body?.Damping ?? Vector2.Zero;
		set => body?.Damping = value;
	}

	/// <inheritdoc/>
	public override void Start()
	{
		var physics = ModuleManager.GetModule<PhysicsModule>();
		body = physics?.World?.CreateBody(Entity);

		if (body != null)
			body.Position = Entity.Transform.Position;
	}

	/// <inheritdoc/>
	public override void Update()
	{
		if (body == null)
			return;

		Entity.Transform.Position = body.Position;
		Entity.Transform.Rotation = body.Orientation;
	}

	/// <inheritdoc/>
	public override void Destroy()
	{
		base.Destroy();
		var physics = ModuleManager.GetModule<PhysicsModule>();

		if (body != null)
			physics?.World?.DestroyBody(body);
	}

	/// <inheritdoc/>
	public int AddBoxCollider(float width, float height, float depth)
	{
		if (body == null)
			return 0;

		int id = body.AddBoxCollider(width, height, depth);

		if (IsStatic)
			Orientation = Entity.Transform.Rotation;

		return id;
	}

	/// <inheritdoc/>
	public int AddSphereCollider(float radius)
	{
		if (body == null)
			return 0;

		int id = body.AddSphereCollider(radius);

		if (IsStatic)
			Orientation = Entity.Transform.Rotation;

		return id;
	}

	/// <inheritdoc/>
	public void RemoveCollider(int id) => body?.RemoveCollider(id);

	/// <inheritdoc/>
	public void AddForce(Vector3 force) => body?.AddForce(force);

	/// <inheritdoc/>
	public event Action<IPhysicsBody>? OnCollisionEnter
	{
		add { if (body != null) body.OnCollisionEnter += value; }
		remove { if (body != null) body.OnCollisionEnter -= value; }
	}

	/// <inheritdoc/>
	public event Action<IPhysicsBody>? OnCollisionExit
	{
		add { if (body != null) body.OnCollisionExit += value; }
		remove { if (body != null) body.OnCollisionExit -= value; }
	}
}
