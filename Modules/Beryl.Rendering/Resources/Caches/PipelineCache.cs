// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;

namespace Beryl.Rendering.Resources.Caches;

/// <summary>
/// Caches graphics pipelines.
/// </summary>
public sealed class PipelineCache : ResourceCache<PipelineCache, PipelineDescriptor, IPipeline>
{
	/// <inheritdoc/>
	protected override IPipeline Create(PipelineDescriptor key) => Renderer.Device.ResourceFactory.CreateGraphicsPipeline(key);
}
