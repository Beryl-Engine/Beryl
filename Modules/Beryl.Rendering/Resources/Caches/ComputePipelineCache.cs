// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.RHI.Resources;

namespace Beryl.Rendering.Resources.Caches;

/// <summary>
/// Caches compute pipelines.
/// </summary>
public sealed class ComputePipelineCache : ResourceCache<ComputePipelineCache, ComputePipelineDescriptor, IComputePipeline>
{
	/// <inheritdoc/>
	protected override IComputePipeline Create(ComputePipelineDescriptor key) => ModuleManager.GetModule<RenderingModule>()!.Device.ResourceFactory.CreateComputePipeline(key);
}
