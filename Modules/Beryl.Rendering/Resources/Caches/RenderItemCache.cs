// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Standard;
using Beryl.Math;

using Beryl.RHI.Resources;

namespace Beryl.Rendering.Resources.Caches;

public sealed class RenderItemCache : ResourceCache<RenderItemCache, RenderItemKey, RenderItem>
{
	/// <inheritdoc/>
	protected override RenderItem Create(RenderItemKey key)
	{
		var factory = RenderingModule.Device.ResourceFactory;

		int vertCount = key.VertexPositions.Length;
		float[] vertexData = new float[vertCount * 8];
		for (int i = 0; i < vertCount; i++)
		{
			vertexData[i * 8 + 0] = key.VertexPositions[i].X;
			vertexData[i * 8 + 1] = key.VertexPositions[i].Y;
			vertexData[i * 8 + 2] = key.VertexPositions[i].Z;
			vertexData[i * 8 + 3] = i < key.VertexNormals.Length ? key.VertexNormals[i].X : 0f;
			vertexData[i * 8 + 4] = i < key.VertexNormals.Length ? key.VertexNormals[i].Y : 0f;
			vertexData[i * 8 + 5] = i < key.VertexNormals.Length ? key.VertexNormals[i].Z : 1f;
			vertexData[i * 8 + 6] = i < key.VertexUVs.Length ? key.VertexUVs[i].X : 0f;
			vertexData[i * 8 + 7] = i < key.VertexUVs.Length ? key.VertexUVs[i].Y : 0f;
		}

		IBuffer vb = factory.CreateBuffer(new BufferDescriptor((uint)vertexData.Length * sizeof(float), BufferUsage.VertexBuffer));
		IBuffer ib = factory.CreateBuffer(new BufferDescriptor((uint)key.Indices.Length * sizeof(uint), BufferUsage.IndexBuffer));

		RenderingModule.Device.UpdateBuffer(vb, 0, vertexData);
		RenderingModule.Device.UpdateBuffer(ib, 0, key.Indices);

		FrameCountedResource<IPipeline> pipeline = PipelineCache.Instance.GetOrCreate(key.Pipeline);

		return new RenderItem(vb, ib, (uint)key.Indices.Length, pipeline);
	}
}

public readonly ref struct RenderItemKey(ReadOnlySpan<Vector3> vertexPositions, ReadOnlySpan<Vector3> vertexNormals, ReadOnlySpan<Vector2> vertexUVs, ReadOnlySpan<uint> indices, PipelineDescriptor pipeline) : IAlwaysHashable
{
	public readonly ReadOnlySpan<Vector3> VertexPositions = vertexPositions;
	public readonly ReadOnlySpan<Vector3> VertexNormals = vertexNormals;
	public readonly ReadOnlySpan<Vector2> VertexUVs = vertexUVs;
	public readonly ReadOnlySpan<uint> Indices = indices;
	public readonly PipelineDescriptor Pipeline = pipeline;

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;

			foreach (var v in VertexPositions)
				hash = hash * 31 + v.GetHashCode();

			foreach (var v in VertexNormals)
				hash = hash * 31 + v.GetHashCode();

			foreach (var v in VertexUVs)
				hash = hash * 31 + v.GetHashCode();

			foreach (uint i in Indices)
				hash = hash * 31 + (int)i;

			hash = hash * 31 + Pipeline.GetHashCode();
			return hash;
		}
	}
}
