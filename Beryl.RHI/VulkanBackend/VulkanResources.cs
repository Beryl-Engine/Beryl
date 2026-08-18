// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;
using Silk.NET.Vulkan;

namespace Beryl.RHI.VulkanBackend;

internal sealed class VulkanFramebuffer(Vk vk) : IFramebuffer
{
	/// <inheritdoc/>
	public void Dispose() => throw new NotImplementedException();
}
