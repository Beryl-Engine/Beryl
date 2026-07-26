// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;
using Beryl.Rendering.Resources;

namespace Beryl.Rendering;

/// <summary>
/// A Renderable object.
/// </summary>
public interface IClientRenderable
{
	Vector3[] Vertices { get; }
	Vector3[] Normals { get; }
	Vector2[] UVs { get; }
	uint[] Indices { get; }
	Material Material { get; }
}
