// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Assimp;

using Beryl.Common.Resources.DefaultProvider;
using Beryl.Math;

namespace Beryl.Rendering.Resources;

/// <summary>
/// A Collection of vertices, indices, normals and UVs that form a renderable object.
/// </summary>
public class Mesh : IClientRenderable
{
	/// <summary> The material to use when rendering this mesh. </summary>
	public Material Material { get; set; } = Material.Default;

	public Vector3[] Vertices { get; set; } = new Vector3[0];
	public uint[] Indices { get; set; } = new uint[0];
	public Vector3[] Normals { get; set; } = new Vector3[0];
	public Vector2[] UVs { get; set; } = new Vector2[0];
}


[ResourceLoader]
internal sealed class MeshLoader : ResourceLoader<Mesh>
{
	/// <inheritdoc/>
	public override Mesh? LoadResource(byte[] data, string? name = null)
	{
		using var ms = new MemoryStream(data);
		using var importer = new AssimpContext();

		var scene = importer.ImportFileFromStream(
			ms,
			PostProcessSteps.Triangulate |
			PostProcessSteps.GenerateNormals |
			PostProcessSteps.FlipUVs |
			PostProcessSteps.JoinIdenticalVertices |
			PostProcessSteps.ImproveCacheLocality |
			PostProcessSteps.FlipWindingOrder
		);

		if (scene == null || scene.MeshCount == 0)
			return null;

		var mesh = scene.Meshes[0];

		var vertices = mesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();
		var normals = mesh.Normals.Select(n => new Vector3(n.X, n.Y, n.Z)).ToArray();
		var indices = mesh.GetIndices();

		Mesh output = new();
		output.Vertices = vertices;
		output.Normals = normals;
		output.Indices = indices.Select(i => unchecked((uint)i)).ToArray();

		return output;
	}
}
