// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)


namespace Beryl.Physics.Components.Colliders;

public interface ICollider
{
	int ShapeID { get; }

	void AddTo(RigidBody rb);

	void RemoveFrom(RigidBody rb)
	{
		if (ShapeID != -1)
			rb.InternalBody?.RemoveCollider(ShapeID);
	}
}
