// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Silk.NET.Vulkan;

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
		public PhysicalDevice GetOptimalDevice(Instance instance)
		{
			PhysicalDevice? optDevice = null;

			IReadOnlyCollection<PhysicalDevice> devices = vk.GetPhysicalDevices(instance);
			foreach (PhysicalDevice device in devices)
			{
				if (vk.GetPhysicalDeviceProperty(device).DeviceType == PhysicalDeviceType.DiscreteGpu)
					optDevice = device;
			}

			if (optDevice == null)
				throw new Exception("Failed to find a Vulkan supported device.");

			return optDevice.Value;
		}
	}
}
