// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Physics.Components;
using Beryl.Physics.Components.Colliders;
using Beryl.Rendering.Components;
using Beryl.Rendering.Resources;
using Beryl.Scenes.Entities;
using System.Collections.ObjectModel;

namespace Beryl.Rendering.Entities;

[Entity("dynamic_prop")]
public class DynamicProp : Entity
{
	public Mesh? Mesh { get => Renderer.Mesh; set => Renderer.Mesh = value; }

	public ObservableCollection<ICollider> Colliders => RigidBody.Colliders;

	public bool IsStatic { get => RigidBody.IsStatic; set => RigidBody.IsStatic = value; }

	public bool HasGravity { get => RigidBody.AffectedByGravity; set => RigidBody.AffectedByGravity = value; }

	[ImplicitComponent]
	protected MeshRenderer Renderer { get; } = null!;

	[ImplicitComponent]
	protected RigidBody RigidBody { get; } = null!;
}
