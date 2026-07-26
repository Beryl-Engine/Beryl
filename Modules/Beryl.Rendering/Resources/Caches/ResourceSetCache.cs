// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;

namespace Beryl.Rendering.Resources.Caches;

public sealed class ResourceSetCache : ResourceCache<ResourceSetCache, ResourceDescriptor, IResourceSet>
{
	/// <inheritdoc/>
	protected override IResourceSet Create(ResourceDescriptor key) => Renderer.Device.ResourceFactory.CreateResourceSet(key);
}
