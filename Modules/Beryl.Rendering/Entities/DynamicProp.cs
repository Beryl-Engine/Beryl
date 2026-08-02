// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Physics.Components;
using Beryl.Rendering.Components;
using Beryl.Scenes.Entities;

namespace Beryl.Rendering.Entities;

[Entity("dynamic_prop")]
public class DynamicProp : Entity
{
	[ImplicitComponent]
	public MeshRenderer Renderer { get; } = null!;

	[ImplicitComponent]
	public RigidBody RigidBody { get; } = null!;
}
