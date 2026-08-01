// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.Rendering.Components;
using Beryl.Rendering.Resources;
using Beryl.Scenes.Entities;

namespace Beryl.Rendering.Entities;

[Entity("skybox")]
public class Skybox : Entity
{
	[ImplicitComponent]
	protected MeshRenderer Renderer { get; } = null!;

	/// <inheritdoc/>
	public override void Start()
	{
		base.Start();

		Mesh? skyMesh = Application.ResourceProvider.GetResource<Mesh>("Assets/PrimitiveCube.fbx");
		Shader? sky = Application.ResourceProvider.GetResource<Shader>("Assets/Defaults/Sky.slang");
		skyMesh?.Material = new(sky ?? throw new Exception("Skybox shader not found."));

		Renderer.Mesh = skyMesh;
	}
}
