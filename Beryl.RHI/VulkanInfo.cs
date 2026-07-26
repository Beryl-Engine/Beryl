// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.RHI;

/// <summary>
/// Low-Level information about a Vulkan instance.
/// </summary>
public readonly struct VulkanInfo
{
	public IntPtr Instance { get; init; }
	public IntPtr PhysicalDevice { get; init; }
	public IntPtr Device { get; init; }

	public uint GraphicsQueueFamilyIndex { get; init; }
}
