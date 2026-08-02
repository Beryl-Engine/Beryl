// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Utility;
using Beryl.Physics.Components;
using Beryl.Physics.Components.Colliders;
using Beryl.Rendering.Components;
using Beryl.Rendering.Resources;
using Beryl.Scenes.Entities;

namespace Beryl.Rendering.Entities;

/// <summary>
/// Internal entity useful for debugging systems.
/// </summary>
[Entity]
public class DebugEntity : Entity
{
	[ImplicitComponent]
	public MeshRenderer Renderer { get; } = null!;

	[ImplicitComponent]
	public RigidBody RigidBody { get; } = null!;

	/// <inheritdoc/>
	public override void Start()
	{
		base.Start();

		RigidBody.IsStatic = true;
		RigidBody.Colliders.Add(new BoxCollider(1f, 1f, 1f));

		Renderer.Mesh = Mesh.Cube;

		BerylConsole.Log("Debug entity started.");
	}
}
