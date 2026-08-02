// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.Math;
using Beryl.Physics.Components.Colliders;
using Beryl.Scenes.Components;
using Beryl.Scenes.Entities;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Beryl.Physics.Components;

/// <summary>
/// Represents a physics rigid body that enables simulation of physical interactions for an <see cref="Entity"/>.
/// </summary>
public class RigidBody : Component
{
	internal IPhysicsBody? InternalBody { get; private set; }

	/// <summary> All <see cref="ICollider"/>s that define the shape of this <see cref="RigidBody"/>. </summary>
	public ObservableCollection<ICollider> Colliders { get; } = new();

	/// <inheritdoc/>
	public bool IsStatic { get => InternalBody?.IsStatic ?? false; set => InternalBody?.IsStatic = value; }

	/// <inheritdoc/>
	public bool AffectedByGravity { get => InternalBody?.AffectedByGravity ?? false; set => InternalBody?.AffectedByGravity = value; }

	/// <inheritdoc/>
	public float Mass { get => InternalBody?.Mass ?? 0f; set => InternalBody?.Mass = value; }

	/// <inheritdoc/>
	public float Friction { get => InternalBody?.Friction ?? 0f; set => InternalBody?.Friction = value; }

	/// <inheritdoc/>
	public float Restitution { get => InternalBody?.Restitution ?? 0f; set => InternalBody?.Restitution = value; }

	/// <inheritdoc/>
	public Vector3 Position { get => InternalBody?.Position ?? Vector3.Zero; set => InternalBody?.Position = value; }

	/// <inheritdoc/>
	public Quaternion Orientation { get => InternalBody?.Orientation ?? new Quaternion(0, 0, 0, 1); set => InternalBody?.Orientation = value; }

	/// <inheritdoc/>
	public Vector3 Velocity { get => InternalBody?.Velocity ?? Vector3.Zero; set => InternalBody?.Velocity = value; }

	/// <inheritdoc/>
	public Vector3 AngularVelocity { get => InternalBody?.AngularVelocity ?? Vector3.Zero; set => InternalBody?.AngularVelocity = value; }

	/// <inheritdoc/>
	public Vector2 Damping { get => InternalBody?.Damping ?? Vector2.Zero; set => InternalBody?.Damping = value; }

	public RigidBody()
	{
		var physics = ModuleManager.GetModule<PhysicsModule>();
		InternalBody = physics?.World.CreateBody(Entity);

		Colliders.CollectionChanged += OnCollidersModified;
	}

	/// <inheritdoc/>
	public override void Start()
	{
		base.Start();

		InternalBody?.Position = Entity.Transform.Position;
	}

	/// <inheritdoc/>
	public override void Update()
	{
		base.Update();

		if (InternalBody == null)
			return;

		Entity.Transform.Position = InternalBody.Position;
		Entity.Transform.Rotation = InternalBody.Orientation;
	}

	/// <inheritdoc/>
	public override void Destroy()
	{
		base.Destroy();

		Colliders.CollectionChanged -= OnCollidersModified;

		foreach (var collider in Colliders)
			collider.RemoveFrom(this);

		var physics = ModuleManager.GetModule<PhysicsModule>();

		if (InternalBody != null)
			physics?.World?.DestroyBody(InternalBody);
	}

	/// <inheritdoc/>
	public void AddForce(Vector3 force) => InternalBody?.AddForce(force);

	private void OnCollidersModified(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Add)
		{
			foreach (ICollider collider in e.NewItems!)
				collider.AddTo(this);
		}

		if (e.Action == NotifyCollectionChangedAction.Remove)
		{
			foreach (ICollider collider in e.OldItems!)
				collider.RemoveFrom(this);
		}
	}
}
