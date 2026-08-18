// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;

namespace Beryl.RHI.VulkanBackend;

// TODO:
// Try and remove all fixed statements

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
		public Result GetQueueFamilyIndex(in PhysicalDevice device, QueueFlags flags, out uint index)
		{
			ReadOnlySpan<QueueFamilyProperties> queueFamilies = vk.GetQueueFamilyProperties(in device);
			foreach (QueueFamilyProperties queueFamily in queueFamilies)
			{
				if ((queueFamily.QueueFlags & flags) == flags)
				{
					index = (uint)queueFamilies.IndexOf(queueFamily);
					return Result.Success;
				}
			}

			index = 0;

			return Result.ErrorUnknown;
		}

		public Result GetPresentQueueFamilyIndex(in Instance instance, in PhysicalDevice physDevice, in SurfaceKHR surface, out uint index)
		{
			if (vk.TryGetInstanceExtension<KhrSurface>(instance, out var khrSurface) == false)
			{
				index = 0;
				return Result.ErrorExtensionNotPresent;
			}

			uint queueFamilyCount = (uint)vk.GetQueueFamilyProperties(in physDevice).Length;

			for (uint i = 0; i < queueFamilyCount; i++)
			{
				khrSurface.GetPhysicalDeviceSurfaceSupport(physDevice, i, surface, out Bool32 supported);
				if (supported)
				{
					index = i;
					return Result.Success;
				}
			}

			index = 0;
			return Result.ErrorUnknown;
		}

		/// <summary> Gets the surface capabilities. </summary>
		public Result GetSurfaceCapabilities(in Instance instance, in PhysicalDevice physDevice, in SurfaceKHR surface, out SurfaceCapabilitiesKHR capabilities)
		{
			if (vk.TryGetInstanceExtension<KhrSurface>(instance, out var khrSurface) == false)
			{
				capabilities = default;
				return Result.ErrorExtensionNotPresent;
			}

			return khrSurface.GetPhysicalDeviceSurfaceCapabilities(physDevice, surface, out capabilities);
		}

		/// <summary> Gets the supported surface formats. </summary>
		public unsafe Result GetSurfaceFormats(in Instance instance, in PhysicalDevice physDevice, in SurfaceKHR surface, out SurfaceFormatKHR[] formats)
		{
			if (vk.TryGetInstanceExtension<KhrSurface>(instance, out var khrSurface) == false)
			{
				formats = [];
				return Result.ErrorExtensionNotPresent;
			}

			uint formatCount = 0;
			khrSurface.GetPhysicalDeviceSurfaceFormats(physDevice, surface, ref formatCount, null);

			formats = new SurfaceFormatKHR[formatCount];
			fixed (SurfaceFormatKHR* pFormats = formats)
				return khrSurface.GetPhysicalDeviceSurfaceFormats(physDevice, surface, &formatCount, pFormats);
		}

		/// <summary> Gets the <see cref="Image"/>s belonging to a swapchain. </summary>
		public unsafe Result GetSwapchainImages(in Instance instance, in Device device, in SwapchainKHR swapchain, out Image[] images)
		{
			if (vk.TryGetDeviceExtension<KhrSwapchain>(instance, device, out var khrSwapchain) == false)
			{
				images = [];
				return Result.ErrorExtensionNotPresent;
			}

			uint imageCount = 0;
			khrSwapchain.GetSwapchainImages(device, swapchain, ref imageCount, null);

			images = new Image[imageCount];
			fixed (Image* pImages = images)
				return khrSwapchain.GetSwapchainImages(device, swapchain, &imageCount, pImages);
		}

		/// <summary> Creates swapchain-compatible <see cref="ImageView"/>s for a set of <see cref="Image"/>s. </summary>
		public unsafe Result CreateSwapchainImageViews(in Device device, in Image[] images, in Format format, out ImageView[] imageViews)
		{
			imageViews = new ImageView[images.Length];

			for (int i = 0; i < images.Length; i++)
			{
				ImageViewCreateInfo createInfo = new()
				{
					SType = StructureType.ImageViewCreateInfo,

					Image = images[i],
					ViewType = ImageViewType.Type2D,
					Format = format,

					Components = new ComponentMapping
					{
						R = ComponentSwizzle.Identity,
						G = ComponentSwizzle.Identity,
						B = ComponentSwizzle.Identity,
						A = ComponentSwizzle.Identity
					},

					SubresourceRange = new ImageSubresourceRange
					{
						AspectMask = ImageAspectFlags.ColorBit,
						BaseMipLevel = 0,
						LevelCount = 1,
						BaseArrayLayer = 0,
						LayerCount = 1
					}
				};

				Result result = vk.CreateImageView(device, in createInfo, null, out imageViews[i]);
				if (result != Result.Success)
					return result;
			}

			return Result.Success;
		}

		/// <summary> Creates an optimal <see cref="SwapchainKHR"/> for the given physical device and surface. </summary>
		public unsafe Result CreateOptimalSwapchain(in Instance instance, in PhysicalDevice physDevice, in Device device, in SurfaceKHR surface, out SwapchainKHR swapchain, out SurfaceFormatKHR swapchainFormat, out Extent2D swapchainExtent)
		{
			swapchain = default;
			swapchainFormat = default;
			swapchainExtent = default;

			if (vk.TryGetInstanceExtension<KhrSurface>(instance, out var khrSurface) == false)
				return Result.ErrorExtensionNotPresent;

			if (vk.TryGetDeviceExtension<KhrSwapchain>(instance, device, out var khrSwapchain) == false)
				return Result.ErrorExtensionNotPresent;

			vk.GetSurfaceCapabilities(in instance, in physDevice, in surface, out SurfaceCapabilitiesKHR capabilities).ThrowIfFailed();
			vk.GetSurfaceFormats(in instance, physDevice, in surface, out SurfaceFormatKHR[] formats).ThrowIfFailed();
			vk.GetSurfacePresentModes(in instance, physDevice, in surface, out PresentModeKHR[] presentModes).ThrowIfFailed();

			vk.GetQueueFamilyIndex(in physDevice, QueueFlags.GraphicsBit, out uint graphicsFamilyIndex).ThrowIfFailed();
			vk.GetPresentQueueFamilyIndex(in instance, in physDevice, in surface, out uint presentFamilyIndex).ThrowIfFailed();

			swapchainFormat = formats.FirstOrDefault(f => f.Format == Format.B8G8R8A8Srgb && f.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr, formats[0]); // Prefer SRGB
			PresentModeKHR presentMode = presentModes.Contains(PresentModeKHR.MailboxKhr) ? PresentModeKHR.MailboxKhr : PresentModeKHR.FifoKhr; // Prefer Mailbox
			swapchainExtent = capabilities.CurrentExtent.Width != uint.MaxValue ? capabilities.CurrentExtent : capabilities.MaxImageExtent;

			uint imageCount = capabilities.MinImageCount + 1;
			if (capabilities.MaxImageCount > 0 && imageCount > capabilities.MaxImageCount)
				imageCount = capabilities.MaxImageCount;

			SwapchainCreateInfoKHR createInfo = new()
			{
				SType = StructureType.SwapchainCreateInfoKhr,

				Surface = surface,
				MinImageCount = imageCount,

				ImageFormat = swapchainFormat.Format,
				ImageColorSpace = swapchainFormat.ColorSpace,
				ImageExtent = swapchainExtent,
				ImageArrayLayers = 1,
				ImageUsage = ImageUsageFlags.ColorAttachmentBit,

				PreTransform = capabilities.CurrentTransform,
				CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
				PresentMode = presentMode,
				Clipped = true,

				OldSwapchain = default
			};

			if (graphicsFamilyIndex != presentFamilyIndex)
			{
				uint* indices = stackalloc uint[] { graphicsFamilyIndex, presentFamilyIndex };

				createInfo.ImageSharingMode = SharingMode.Concurrent;
				createInfo.QueueFamilyIndexCount = 2;
				createInfo.PQueueFamilyIndices = indices;
			}
			else
			{
				createInfo.ImageSharingMode = SharingMode.Exclusive;
			}

			return khrSwapchain.CreateSwapchain(device, in createInfo, null, out swapchain);
		}

		/// <summary> Gets the supported present modes for a physical device and surface. </summary>
		public unsafe Result GetSurfacePresentModes(in Instance instance, in PhysicalDevice physDevice, in SurfaceKHR surface, out PresentModeKHR[] presentModes)
		{
			if (vk.TryGetInstanceExtension<KhrSurface>(instance, out var khrSurface) == false)
			{
				presentModes = [];
				return Result.ErrorExtensionNotPresent;
			}

			uint modeCount = 0;
			khrSurface.GetPhysicalDeviceSurfacePresentModes(physDevice, surface, ref modeCount, null);

			presentModes = new PresentModeKHR[modeCount];
			fixed (PresentModeKHR* pModes = presentModes)
				khrSurface.GetPhysicalDeviceSurfacePresentModes(physDevice, surface, &modeCount, pModes);

			return Result.Success;
		}

		/// <summary> Gets the properties of all queue families. </summary>
		public unsafe ReadOnlySpan<QueueFamilyProperties> GetQueueFamilyProperties(in PhysicalDevice physDevice)
		{
			uint queueFamilyCount = 0;
			vk.GetPhysicalDeviceQueueFamilyProperties(physDevice, ref queueFamilyCount, null);

			var queueFamilies = new QueueFamilyProperties[queueFamilyCount];
			vk.GetPhysicalDeviceQueueFamilyProperties(physDevice, &queueFamilyCount, queueFamilies);

			return queueFamilies;
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
