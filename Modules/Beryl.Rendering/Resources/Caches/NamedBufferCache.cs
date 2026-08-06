// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)


using Beryl.RHI.Resources;

namespace Beryl.Rendering.Resources.Caches;

public sealed class NamedBufferCache : ResourceCache<NamedBufferCache, NamedBufferDescriptor, IBuffer>
{
	/// <inheritdoc/>
	protected override IBuffer Create(NamedBufferDescriptor key) => RenderingModule.Device.ResourceFactory.CreateBuffer(new BufferDescriptor(key.Size, key.Usage));
}
