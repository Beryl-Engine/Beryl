// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;


namespace Beryl.Rendering.Resources.Caches;

public sealed class SamplerCache : ResourceCache<SamplerCache, SamplerDescriptor, ISampler>
{
	/// <inheritdoc/>
	protected override ISampler Create(SamplerDescriptor key) => RenderingModule.Device.ResourceFactory.CreateSampler(key);
}
