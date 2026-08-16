// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Utility;
using Beryl.RHI.Resources;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

namespace Beryl.RHI.VulkanBackend;

internal sealed class VulkanDevice : IGraphicsDevice
{
	internal Vk Vk { get; } = Vk.GetApi();

	/// <inheritdoc/>
	public VulkanInfo? VulkanInfo { get; private set; }

	/// <inheritdoc/>
	public DeviceFeatures Features { get; } = DeviceFeatures.ClipSpaceYInverted;

	/// <inheritdoc/>
	public IFramebuffer SwapchainFramebuffer => throw new NotImplementedException();

	/// <inheritdoc/>
	public IResourceFactory ResourceFactory => throw new NotImplementedException();

	/// <inheritdoc/>
	public unsafe void Initialize(IWindow window)
	{
		if (window.VkSurface == null)
			throw new ArgumentException("Window does not have a Vulkan surface.");

		nint name = SilkMarshal.StringToPtr("Beryl");

		try
		{
			ApplicationInfo appInfo = new()
			{
				SType = StructureType.ApplicationInfo,

				PApplicationName = (byte*)name,
				ApplicationVersion = new Version32(1, 0, 0),

				PEngineName = (byte*)name,
				EngineVersion = new Version32(1, 0, 0),

				ApiVersion = Vk.Version13
			};

			byte** windowExtensions = window.VkSurface.GetRequiredExtensions(out uint count);

			string[] instanceExtensions =
			[
				..SilkMarshal.PtrToStringArray((nint)windowExtensions, (int)count),
				"VK_EXT_debug_utils",
			];

			if (Vk.IsLayerAvailable("VK_LAYER_KHRONOS_validation") == false)
				throw new Exception("Vulkan validation layer is not available."); // Hack

			string[] instanceLayers =
			[
				"VK_LAYER_KHRONOS_validation"
			];

			byte** instanceExtensionsPtr = (byte**)SilkMarshal.StringArrayToPtr(instanceExtensions);
			byte** instanceLayersPtr = (byte**)SilkMarshal.StringArrayToPtr(instanceLayers);

			InstanceCreateInfo instanceCreateInfo = new()
			{
				SType = StructureType.InstanceCreateInfo,

				PApplicationInfo = &appInfo,
				
				EnabledExtensionCount = (uint)instanceExtensions.Length,
				PpEnabledExtensionNames = instanceExtensionsPtr,

				EnabledLayerCount = (uint)instanceLayers.Length,
				PpEnabledLayerNames = instanceLayersPtr
			};

			Vk.CreateInstance(in instanceCreateInfo, null, out Instance instance).ThrowIfFailed();
			Vk.CreateDebugCallback(in instance, (severity, type, message) => BerylConsole.Log($"[{severity}] [{type}] {message}", "Vulkan")).ThrowIfFailed();
			Vk.SubmitDebugMessage(in instance, DebugUtilsMessageSeverityFlagsEXT.InfoBitExt, DebugUtilsMessageTypeFlagsEXT.GeneralBitExt, "Initialized Vulkan Debug callback.");

			Vk.GetOptimalPhysicalDevice(in instance, out PhysicalDevice physDevice).ThrowIfFailed();

			Vk.GetWindowSurface(in instance, in window, out SurfaceKHR surface).ThrowIfFailed();

			Vk.GetQueueFamilyIndex(in physDevice, QueueFlags.GraphicsBit, out uint graphicsFamilyIndex).ThrowIfFailed();
			Vk.GetPresentQueueFamilyIndex(in instance, in physDevice, in surface, out uint presentFamilyIndex).ThrowIfFailed();

			// Merge duplicates
			// This is pretty yuck
			HashSet<uint> queueFamilyIndices = new()
			{
				graphicsFamilyIndex,
				presentFamilyIndex
			};

			DeviceQueueCreateInfo* pQueueCreateInfos = stackalloc DeviceQueueCreateInfo[queueFamilyIndices.Count];
			float* pQueuePriorities = stackalloc float[queueFamilyIndices.Count];

			int i = 0;
			foreach (uint familyIndex in queueFamilyIndices)
			{
				pQueuePriorities[i] = 1.0f;
				pQueueCreateInfos[i] = new()
				{
					SType = StructureType.DeviceQueueCreateInfo,
					QueueFamilyIndex = familyIndex,
					QueueCount = 1,
					PQueuePriorities = &pQueuePriorities[i]
				};
				i++;
			}

			PhysicalDeviceFeatures features = new();

			DeviceCreateInfo deviceCreateInfo = new()
			{
				SType = StructureType.DeviceCreateInfo,

				PQueueCreateInfos = pQueueCreateInfos,
				QueueCreateInfoCount = (uint)queueFamilyIndices.Count,

				PEnabledFeatures = &features
			};

			Vk.CreateDevice(physDevice, in deviceCreateInfo, null, out Device device).ThrowIfFailed();

			Queue graphicsQueue = Vk.GetDeviceQueue(device, graphicsFamilyIndex, 0);
			Queue presentQueue = Vk.GetDeviceQueue(device, presentFamilyIndex, 0);

			SilkMarshal.Free((nint)instanceExtensionsPtr);
			SilkMarshal.Free((nint)instanceLayersPtr);

			VulkanInfo = new()
			{
				Instance = instance,
				PhysicalDevice = physDevice,
				Device = device,
				Surface = surface,

				GraphicsQueueFamilyIndex = graphicsFamilyIndex,
				GraphicsQueue = graphicsQueue,

				PresentQueueFamilyIndex = presentFamilyIndex,
				PresentQueue = presentQueue
			};
		}
		finally
		{
			SilkMarshal.FreeString(name);
		}
	}

	/// <inheritdoc/>
	public void ResizeSwapchain(uint width, uint height) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SubmitCommands(ICommandBuffer buffer) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SwapBuffers() => throw new NotImplementedException();

	/// <inheritdoc/>
	public void UpdateBuffer<T>(IBuffer buffer, uint offset, ReadOnlySpan<T> data) where T : unmanaged => throw new NotImplementedException();

	/// <inheritdoc/>
	public unsafe void Dispose()
	{
		if (VulkanInfo == null)
			return;

		Vk.DestroyDevice(VulkanInfo.Value.Device, null);
		Vk.DestroyDebugCallback(VulkanInfo.Value.Instance);
		Vk.DestroyInstance(VulkanInfo.Value.Instance, null);
		Vk.Dispose();
	}
}
