// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Rendering.Components;
using Beryl.Scenes.Entities;

namespace Beryl.Rendering.Entities;

[Entity]
public class Prop : Entity
{
	[ImplicitComponent]
	public MeshRenderer Renderer { get; } = null!;
}
