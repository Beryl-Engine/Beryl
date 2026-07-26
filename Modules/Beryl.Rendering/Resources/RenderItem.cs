// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;

namespace Beryl.Rendering.Resources;

/// <summary>
/// Low-Level representation of a rendered object.
/// </summary>
public sealed class RenderItem : IDisposable
{
	public IBuffer VertexBuffer { get; }
	public IBuffer IndexBuffer { get; }
	public uint IndexCount { get; }
	public FrameCountedResource<IPipeline> Pipeline { get; }

	public RenderItem(IBuffer vertexBuffer, IBuffer indexBuffer, uint indexCount, FrameCountedResource<IPipeline> pipeline)
	{
		VertexBuffer = vertexBuffer;
		IndexBuffer = indexBuffer;
		IndexCount = indexCount;
		Pipeline = pipeline;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		VertexBuffer.Dispose();
		IndexBuffer.Dispose();
	}
}
