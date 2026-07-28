// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;

namespace Beryl.Physics.Components.Colliders;

public struct BoxCollider : ICollider
{
	/// <inheritdoc/>
	public int ShapeID { get; private set; }

	public float Width { get; set; }
	public float Height { get; set; }
	public float Depth { get; set; }

	public BoxCollider(float width, float height, float depth) => (Width, Height, Depth) = (width, height, depth);

	public BoxCollider(Vector3 size) => (Width, Height, Depth) = (size.X, size.Y, size.Z);

	public BoxCollider() : this(1f, 1f, 1f) {}

	/// <inheritdoc/>
	public void AddTo(RigidBody rb) => ShapeID = rb.InternalBody.AddBoxCollider(Width, Height, Depth);
}
