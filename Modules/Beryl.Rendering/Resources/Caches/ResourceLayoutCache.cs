// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;

namespace Beryl.Rendering.Resources.Caches;

public sealed class ResourceLayoutCache : ResourceCache<ResourceLayoutCache, ResourceLayoutDescriptor, IResourceLayout>
{
	/// <inheritdoc/>
	protected override IResourceLayout Create(ResourceLayoutDescriptor key) => Renderer.Device.ResourceFactory.CreateResourceLayout(key);
}
