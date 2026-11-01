// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Assimp;
using Beryl.Common.Resources;
using Beryl.Common.Resources.DefaultProvider;
using Beryl.Math;
using Beryl.Common.Utility;
using Beryl.RHI.Resources;
using System.Collections.Immutable;

namespace Beryl.Rendering.Resources;

/// <summary>
/// A Collection of vertices, indices, normals and UVs that form a renderable object.
/// </summary>
public class Mesh : Resource, IClientRenderable
{
	/// <summary> The material to use when rendering this mesh. </summary>
	public Material Material { get; set; } = Material.Default;

	/// <summary> Imported parts with their own geometry and material. </summary>
	/// <remarks> Empty for single meshes. </remarks>
	public ImmutableArray<Mesh> SubMeshes { get; internal set; } = [];

	public ImmutableArray<Vector3> Vertices
	{
		get;
		set
		{
			field = value;
			ID = Guid.NewGuid();
		}
	} = [];

	public ImmutableArray<uint> Indices
	{
		get;
		set
		{
			field = value;
			ID = Guid.NewGuid();
		}
	} = [];

	public ImmutableArray<Vector3> Normals
	{
		get;
		set
		{
			field = value;
			ID = Guid.NewGuid();
		}
	} = [];

	public ImmutableArray<Vector2> UVs
	{
		get;
		set
		{
			field = value;
			ID = Guid.NewGuid();
		}
	} = [];

	#region Primitives
	/// <summary> One-sided Plane Primitive. </summary>
	public static Mesh Plane => new()
	{
		Vertices = [
			new(-0.5f, 0.0f, -0.5f),
			new( 0.5f, 0.0f, -0.5f),
			new( 0.5f, 0.0f,  0.5f),
			new(-0.5f, 0.0f,  0.5f)
		],
		Normals = [
			Vector3.Up, Vector3.Up, Vector3.Up, Vector3.Up
		],
		UVs = [
			new(0.0f, 1.0f),
			new(1.0f, 1.0f),
			new(1.0f, 0.0f),
			new(0.0f, 0.0f)
		],
		Indices = [
			0, 2, 1,
			0, 3, 2
		]
	};

	/// <summary> Cube/Box Primitive. </summary>
	public static Mesh Cube => new()
	{
		Vertices = [
			new(-0.5f, -0.5f,  0.5f), new( 0.5f, -0.5f,  0.5f), new( 0.5f,  0.5f,  0.5f), new(-0.5f,  0.5f,  0.5f),
			new( 0.5f, -0.5f, -0.5f), new(-0.5f, -0.5f, -0.5f), new(-0.5f,  0.5f, -0.5f), new( 0.5f,  0.5f, -0.5f),
			new(-0.5f,  0.5f,  0.5f), new( 0.5f,  0.5f,  0.5f), new( 0.5f,  0.5f, -0.5f), new(-0.5f,  0.5f, -0.5f),
			new(-0.5f, -0.5f, -0.5f), new( 0.5f, -0.5f, -0.5f), new( 0.5f, -0.5f,  0.5f), new(-0.5f, -0.5f,  0.5f),
			new( 0.5f, -0.5f,  0.5f), new( 0.5f, -0.5f, -0.5f), new( 0.5f,  0.5f, -0.5f), new( 0.5f,  0.5f,  0.5f),
			new(-0.5f, -0.5f, -0.5f), new(-0.5f, -0.5f,  0.5f), new(-0.5f,  0.5f,  0.5f), new(-0.5f,  0.5f, -0.5f)
		],
		Normals = [
			Vector3.Forward, Vector3.Forward, Vector3.Forward, Vector3.Forward,
			-Vector3.Forward, -Vector3.Forward, -Vector3.Forward, -Vector3.Forward,
			Vector3.Up, Vector3.Up, Vector3.Up, Vector3.Up,
			-Vector3.Up, -Vector3.Up, -Vector3.Up, -Vector3.Up,
			Vector3.Right, Vector3.Right, Vector3.Right, Vector3.Right,
			-Vector3.Right, -Vector3.Right, -Vector3.Right, -Vector3.Right
		],
		UVs = [
			new(0.0f, 0.0f), new(1.0f, 0.0f), new(1.0f, 1.0f), new(0.0f, 1.0f),
			new(0.0f, 0.0f), new(1.0f, 0.0f), new(1.0f, 1.0f), new(0.0f, 1.0f),
			new(0.0f, 0.0f), new(1.0f, 0.0f), new(1.0f, 1.0f), new(0.0f, 1.0f),
			new(0.0f, 0.0f), new(1.0f, 0.0f), new(1.0f, 1.0f), new(0.0f, 1.0f),
			new(0.0f, 0.0f), new(1.0f, 0.0f), new(1.0f, 1.0f), new(0.0f, 1.0f),
			new(0.0f, 0.0f), new(1.0f, 0.0f), new(1.0f, 1.0f), new(0.0f, 1.0f)
		],
		Indices = [
			0, 1, 2,   0, 2, 3,
			4, 5, 6,   4, 6, 7,
			8, 9, 10,  8, 10, 11,
			12, 13, 14, 12, 14, 15,
			16, 17, 18, 16, 18, 19,
			20, 21, 22, 20, 22, 23
		]
	};
	#endregion
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
			PostProcessSteps.PreTransformVertices
		);

		if (scene == null || scene.MeshCount == 0)
			return null;

		List<Vector3> vertices = new();
		List<Vector3> normals = new();
		List<Vector2> uvs = new();
		List<uint> indices = new();
		var parts = ImmutableArray.CreateBuilder<Mesh>();
		var materials = new Dictionary<int, Material>();
		var textures = new Dictionary<string, Texture>(StringComparer.Ordinal);
		Texture white = Texture.White;

		foreach (var mesh in scene.Meshes)
		{
			if (!materials.TryGetValue(mesh.MaterialIndex, out Material? material))
			{
				material = new Material(Material.Default.Shader);
				material.SetProperty("SpecularIntensity", new ShaderValue.Float(0));
				material.SetProperty("DiffuseMap", white);

				if (mesh.MaterialIndex >= 0 && mesh.MaterialIndex < scene.MaterialCount)
				{
					var source = scene.Materials[mesh.MaterialIndex];
					var color = source.ColorDiffuse;
					material.SetProperty("Color", new ShaderValue.Vector(color.X, color.Y, color.Z));

					if (source.GetMaterialTexture(TextureType.BaseColor, 0, out TextureSlot slot) || source.GetMaterialTexture(TextureType.Diffuse, 0, out slot))
					{
						if (!textures.TryGetValue(slot.FilePath, out Texture? texture))
						{
							texture = LoadEmbeddedTexture(scene, slot.FilePath) ?? white;
							textures.Add(slot.FilePath, texture);
						}

						material.SetProperty("DiffuseMap", texture);
					}
				}

				materials.Add(mesh.MaterialIndex, material);
			}

			var part = new Mesh
			{
				Material = material,
				Vertices = mesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToImmutableArray(),
				Normals = Enumerable.Range(0, mesh.VertexCount).Select(i => mesh.HasNormals ? new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z) : Vector3.Forward).ToImmutableArray(),
				UVs = Enumerable.Range(0, mesh.VertexCount).Select(i => mesh.HasTextureCoords(0) ? new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y) : Vector2.Zero).ToImmutableArray(),
				Indices = mesh.GetIndices().Select(i => checked((uint)i)).ToImmutableArray()
			};

			parts.Add(part);

			uint vertexOffset = checked((uint)vertices.Count);
			vertices.AddRange(part.Vertices);
			normals.AddRange(part.Normals);
			uvs.AddRange(part.UVs);

			indices.AddRange(part.Indices.Select(i => checked(i + vertexOffset)));
		}

		if (parts.Count == 1)
			return parts[0];

		Mesh output = new();
		output.Material = parts[0].Material;
		output.SubMeshes = parts.ToImmutable();
		output.Vertices = vertices.ToImmutableArray();
		output.Normals = normals.ToImmutableArray();
		output.UVs = uvs.ToImmutableArray();
		output.Indices = indices.ToImmutableArray();

		return output;
	}

	private static Texture? LoadEmbeddedTexture(Scene scene, string path)
	{
		var embedded = scene.GetEmbeddedTexture(path);
		if (embedded == null)
		{
			BerylConsole.Warning($"Diffuse texture '{path}' is not embedded in the mesh; using white.");
			return null;
		}

		if (embedded.IsCompressed)
			return new TextureLoader().LoadResource(embedded.CompressedData);

		byte[] pixels = new byte[checked(embedded.Width * embedded.Height * 4)];
		for (int i = 0; i < embedded.NonCompressedData.Length; i++)
		{
			var texel = embedded.NonCompressedData[i];
			pixels[i * 4] = texel.R;
			pixels[i * 4 + 1] = texel.G;
			pixels[i * 4 + 2] = texel.B;
			pixels[i * 4 + 3] = texel.A;
		}
		return new Texture(embedded.Width, embedded.Height, TextureFormat.RGBA8, pixels);
	}
}
