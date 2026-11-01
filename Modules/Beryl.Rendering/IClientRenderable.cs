// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;
using Beryl.Rendering.Resources;
using System.Collections.Immutable;

namespace Beryl.Rendering;

/// <summary>
/// A Renderable object.
/// </summary>
public interface IClientRenderable
{
	ImmutableArray<Vector3> Vertices { get; }
	ImmutableArray<Vector3> Normals { get; }
	ImmutableArray<Vector2> UVs { get; }
	ImmutableArray<uint> Indices { get; }
	Material Material { get; }
}
