// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Rendering.Resources.Caches;

using Beryl.RHI.Resources;

public sealed class TextureViewCache : ResourceCache<TextureViewCache, TextureViewDescriptor, ITextureView>
{
	/// <inheritdoc/>
	protected override ITextureView Create(TextureViewDescriptor key) => Renderer.Device.ResourceFactory.CreateTextureView(key);
}
