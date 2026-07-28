// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Physics.Components.Colliders;

public struct SphereCollider : ICollider
{
	/// <inheritdoc/>
	public int ShapeID { get; private set; }

	public float Radius { get; set; }

	public SphereCollider(float radius) => Radius = radius;

	public SphereCollider() : this(1f) {}

	/// <inheritdoc/>
	public void AddTo(RigidBody rb) => ShapeID = rb.InternalBody.AddSphereCollider(Radius);
}
