// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Rendering.Resources.Caches;

using Beryl.Common;
using Beryl.RHI.Resources;

public sealed class TextureCache : ResourceCache<TextureCache, TextureDescriptor, ITexture>
{
	/// <inheritdoc/>
	protected override ITexture Create(TextureDescriptor key) => ModuleManager.GetModule<RenderingModule>()!.Device.ResourceFactory.CreateTexture(key);
}
