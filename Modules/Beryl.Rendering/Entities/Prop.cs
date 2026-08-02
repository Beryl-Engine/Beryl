// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Physics.Components;
using Beryl.Rendering.Components;
using Beryl.Scenes.Entities;

namespace Beryl.Rendering.Entities;

public enum PropPhysicsMode
{
	Static,
	Dynamic
}

[Entity]
public class Prop : Entity
{
	/// <summary> The <see cref="PropPhysicsMode"/> used to drive <see cref="RigidBody"/> behavior. </summary>
	public PropPhysicsMode PhysicsMode
	{
		get => field;
		set
		{
			field = value;

			switch (value)
			{
				case PropPhysicsMode.Static:
					RigidBody.IsStatic = true;
					RigidBody.AffectedByGravity = false;
					break;
				case PropPhysicsMode.Dynamic:
					RigidBody.IsStatic = false;
					RigidBody.AffectedByGravity = true;
					break;
			}
		}
	}

	[ImplicitComponent]
	public MeshRenderer Renderer { get; } = null!;

	[ImplicitComponent]
	public RigidBody RigidBody { get; private set; } = null!;
}
