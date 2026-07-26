// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Scenes.Components;

namespace Beryl.Physics.Components.Colliders;

public class Collider : Component
{
	protected int shapeId = -1;

	public override void Destroy()
	{
		var rb = Entity.GetComponent<RigidBody>();
		if (rb != null && shapeId != -1)
			rb.RemoveCollider(shapeId);
	}
}
