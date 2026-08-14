// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Silk.NET.Core.Contexts;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

namespace Beryl.RHI.VulkanBackend;

/// <summary>
/// Extensions for Silk.NET.Vulkan.
/// </summary>
internal static class VulkanExtensions
{
	extension(Result res)
	{
		/// <summary> Throws an exception if the result is not <see cref="Result.Success"/>. </summary>
		/// <param name="ex"> The exception to throw. </param>
		public Result ThrowIfFailed(Exception? ex = null)
		{
			if (res != Result.Success)
				throw ex ?? new Exception($"Failed to perform Vulkan operation: {res}.");

			return res;
		}
	}

	extension (Vk vk)
	{
		/// <summary> Gets the best <see cref="PhysicalDevice"/> available for the current system. </summary>
		public Result GetOptimalPhysicalDevice(in Instance instance, out PhysicalDevice optDevice)
		{
			// This is obviously dumb
			optDevice = default;

			IReadOnlyCollection<PhysicalDevice> devices = vk.GetPhysicalDevices(instance);
			foreach (PhysicalDevice device in devices)
			{
				if (vk.GetPhysicalDeviceProperty(device).DeviceType == PhysicalDeviceType.DiscreteGpu)
				{
					optDevice = device;
					return Result.Success;
				}
			}

			foreach (PhysicalDevice device in devices)
			{
				if (vk.GetPhysicalDeviceProperty(device).DeviceType == PhysicalDeviceType.IntegratedGpu)
				{
					optDevice = device;
					return Result.Success;
				}
			}

			return Result.ErrorUnknown;
		}

		/// <summary> Gets the index of the requested queue family. </summary>
		public unsafe Result GetQueueFamilyIndex(in PhysicalDevice device, QueueFlags flags, out uint index)
		{
			uint queueFamilyCount = 0;
			vk.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilyCount, null);

			var queueFamilies = new QueueFamilyProperties[queueFamilyCount];
			vk.GetPhysicalDeviceQueueFamilyProperties(device, &queueFamilyCount, queueFamilies);

			for (uint i = 0; i < queueFamilyCount; i++)
			{
				if ((queueFamilies[i].QueueFlags & flags) == flags)
				{
					index = i;
					return Result.Success;
				}
			}

			index = 0;
			return Result.ErrorUnknown;
		}

		/// <summary> Gets a <see cref="SurfaceKHR"/> for a Silk.NET <see cref="IWindow"/>. </summary>
		public unsafe Result GetWindowSurface(in Instance instance, in IWindow window, out SurfaceKHR surface)
		{
			IVkSurface? winSurface = window.VkSurface;

			if (winSurface == null)
			{
				surface = default;
				return Result.ErrorUnknown;
			}

			surface = new SurfaceKHR(winSurface.Create<AllocationCallbacks>(instance.ToHandle(), null).Handle);

			return Result.Success;
		}
	}
}
