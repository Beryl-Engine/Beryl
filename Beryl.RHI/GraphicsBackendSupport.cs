// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Vortice.Direct3D;
using Vortice.Direct3D12;
using Vortice.DXGI;
using Vortice.Vulkan;

namespace Beryl.RHI;

internal static class GraphicsBackendSupport
{
	public static bool IsDirect3D12Supported()
	{
		if (DXGI.CreateDXGIFactory1<IDXGIFactory1>(out var factory).Failure || factory == null)
			return false;

		using (factory)
		{
			for (uint index = 0; factory.EnumAdapters1(index, out var adapter).Success; index++)
			{
				using (adapter)
				{
					if ((adapter.Description1.Flags & AdapterFlags.Software) == 0 && D3D12.IsSupported(adapter, FeatureLevel.Level_11_0))
						return true;
				}
			}
		}

		return false;
	}

	public static unsafe bool IsVulkanSupported()
	{
		if (Vulkan.vkInitialize() != VkResult.Success || Vulkan.vkEnumerateInstanceVersion() < VkVersion.Version_1_1)
			return false;

		if (Vulkan.vkEnumerateInstanceExtensionProperties(out uint extensionCount) != VkResult.Success || extensionCount == 0)
			return false;

		VkExtensionProperties[] extensions = new VkExtensionProperties[extensionCount];
		if (Vulkan.vkEnumerateInstanceExtensionProperties(extensions) != VkResult.Success ||
			!HasExtension(extensions, "VK_KHR_surface\0"u8) ||
			!(OperatingSystem.IsWindows()
				? HasExtension(extensions, "VK_KHR_win32_surface\0"u8)
				: HasExtension(extensions, "VK_KHR_xlib_surface\0"u8) || HasExtension(extensions, "VK_KHR_wayland_surface\0"u8)))
		{
			return false;
		}

		VkApplicationInfo application = new() { apiVersion = VkVersion.Version_1_1 };
		VkInstanceCreateInfo description = new() { pApplicationInfo = &application };
		if (Vulkan.vkCreateInstance(in description, out VkInstance instance) != VkResult.Success)
			return false;

		var api = new VkInstanceApi(in instance);
		try
		{
			if (api.vkEnumeratePhysicalDevices(out uint deviceCount) != VkResult.Success || deviceCount == 0)
				return false;

			VkPhysicalDevice[] devices = new VkPhysicalDevice[deviceCount];
			if (api.vkEnumeratePhysicalDevices(devices) != VkResult.Success)
				return false;

			foreach (var device in devices)
			{
				if (device.IsNotNull && api.vkGetPhysicalDeviceProperties(device).apiVersion >= VkVersion.Version_1_1 && HasRequiredQueue(api, device) && HasSwapchainSupport(api, device))
					return true;
			}
			return false;
		}
		finally
		{
			api.vkDestroyInstance();
		}
	}

	private static bool HasRequiredQueue(VkInstanceApi api, VkPhysicalDevice device)
	{
		api.vkGetPhysicalDeviceQueueFamilyProperties(device, out uint count);
		VkQueueFamilyProperties[] queues = new VkQueueFamilyProperties[count];

		api.vkGetPhysicalDeviceQueueFamilyProperties(device, queues);

		const VkQueueFlags required = VkQueueFlags.Graphics | VkQueueFlags.Compute;
		foreach (var queue in queues)
		{
			if (queue.queueCount > 0 && (queue.queueFlags & required) == required)
				return true;
		}

		return false;
	}

	private static bool HasSwapchainSupport(VkInstanceApi api, VkPhysicalDevice device)
	{
		if (api.vkEnumerateDeviceExtensionProperties(device, out uint count) != VkResult.Success || count == 0)
			return false;

		VkExtensionProperties[] extensions = new VkExtensionProperties[count];
		return api.vkEnumerateDeviceExtensionProperties(device, extensions) == VkResult.Success && HasExtension(extensions, "VK_KHR_swapchain\0"u8);
	}

	private static unsafe bool HasExtension(ReadOnlySpan<VkExtensionProperties> extensions, ReadOnlySpan<byte> name)
	{
		foreach (var extension in extensions)
		{
			if (new ReadOnlySpan<byte>(extension.extensionName, 256).StartsWith(name))
				return true;
		}

		return false;
	}
}
