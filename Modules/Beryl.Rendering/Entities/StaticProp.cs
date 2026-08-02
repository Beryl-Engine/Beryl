// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Rendering.Components;
using Beryl.Rendering.Resources;
using Beryl.Scenes.Entities;

namespace Beryl.Rendering.Entities;

[Entity("static_prop")]
public class StaticProp : Entity
{
	public Mesh? Mesh { get => Renderer.Mesh; set => Renderer.Mesh = value; }

	[ImplicitComponent]
	protected MeshRenderer Renderer { get; } = null!;
}
