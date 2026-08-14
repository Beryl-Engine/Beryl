// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Windowing;

namespace Beryl.RHI.VulkanBackend;

/// <summary>
/// Extensions for Silk.NET.Vulkan.
/// </summary>
internal static class VulkanExtensions
{
	private static DebugUtilsMessengerCallbackFunctionEXT? debugCallback;
	private static ExtDebugUtils? debugUtils;
	private static DebugUtilsMessengerEXT debugMessenger;

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
		/// <summary> Sets a debug message callback. </summary>
		/// <remarks> This will only work if <paramref name="instance"/> has the <c>VK_EXT_debug_utils</c> extension and the <c>VK_LAYER_KHRONOS_validation</c> layer. </remarks>
		public unsafe Result CreateDebugCallback(in Instance instance, Action<DebugUtilsMessageSeverityFlagsEXT, DebugUtilsMessageTypeFlagsEXT, string> callback)
		{
			debugCallback = (severity, types, data, _) =>
			{
				string? message = SilkMarshal.PtrToString((nint)data->PMessage);

				callback(severity, types, message ?? "ERROR");

				return Vk.False;
			};

			DebugUtilsMessengerCreateInfoEXT debugCreateInfo = new()
			{
				SType = StructureType.DebugUtilsMessengerCreateInfoExt,

				MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt | DebugUtilsMessageSeverityFlagsEXT.WarningBitExt | DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt | DebugUtilsMessageSeverityFlagsEXT.InfoBitExt,
				MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt | DebugUtilsMessageTypeFlagsEXT.ValidationBitExt | DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt,

				PfnUserCallback = debugCallback
			};

			if (vk.TryGetInstanceExtension(instance, out debugUtils) == false)
				return Result.ErrorExtensionNotPresent;

			debugUtils?.CreateDebugUtilsMessenger(instance, in debugCreateInfo, null, out debugMessenger);

			return Result.Success;
		}

		/// <summary> Submits a debug message to the callback set with <see cref="CreateDebugCallback"/>. </summary>
		public unsafe void SubmitDebugMessage(in Instance instance, DebugUtilsMessageSeverityFlagsEXT severity, DebugUtilsMessageTypeFlagsEXT type, string message)
		{
			DebugUtilsMessengerCallbackDataEXT messageData = new()
			{
				SType = StructureType.DebugUtilsMessengerCallbackDataExt,
				PMessage = (byte*)SilkMarshal.StringToPtr(message)
			};

			debugUtils?.SubmitDebugUtilsMessage(instance, severity, type, in messageData);

			SilkMarshal.FreeString((nint)messageData.PMessage);
		}

		/// <summary> Cleans up the current debug callback. </summary>
		public unsafe void DestroyDebugCallback(in Instance instance)
		{
			if (debugUtils == null)
				return;

			debugUtils.DestroyDebugUtilsMessenger(instance, debugMessenger, null);
			debugUtils.Dispose();

			debugCallback = null;
			debugUtils = null;
		}

		/// <summary> Checks if a layer is available. </summary>
		public unsafe bool IsLayerAvailable(string layerName)
		{
			uint layerCount = 0;
			vk.EnumerateInstanceLayerProperties(ref layerCount, null);

			Span<LayerProperties> availableLayers = stackalloc LayerProperties[(int)layerCount];
			vk.EnumerateInstanceLayerProperties(&layerCount, availableLayers);

			bool found = false;
			foreach (var layer in availableLayers)
			{
				string? name = SilkMarshal.PtrToString((nint)layer.LayerName);
				if (name == layerName)
					found = true;
			}

			return found;
		}

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
