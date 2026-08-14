// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Silk.NET.Vulkan;

namespace Beryl.RHI;

/// <summary>
/// Low-Level information about a Vulkan instance.
/// </summary>
public readonly struct VulkanInfo
{
	public Instance Instance { get; init; }
	public PhysicalDevice PhysicalDevice { get; init; }
	public Device Device { get; init; }
	public SurfaceKHR Surface { get; init; }

	public uint GraphicsQueueFamilyIndex { get; init; }
	public Queue GraphicsQueue { get; init; }
}
