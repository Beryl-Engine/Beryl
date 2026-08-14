// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

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

			InstanceCreateInfo instanceCreateInfo = new()
			{
				SType = StructureType.InstanceCreateInfo,

				PApplicationInfo = &appInfo,
				
				EnabledExtensionCount = count,
				PpEnabledExtensionNames = windowExtensions,
				EnabledLayerCount = 0,
			};

			Vk.CreateInstance(in instanceCreateInfo, null, out Instance instance).ThrowIfFailed();

			Vk.GetOptimalPhysicalDevice(in instance, out PhysicalDevice physDevice).ThrowIfFailed();

			Vk.GetQueueFamilyIndex(in physDevice, QueueFlags.GraphicsBit, out uint graphicsFamilyIndex).ThrowIfFailed();

			float priority = 1.0f;
			DeviceQueueCreateInfo queueCreateInfo = new()
			{
				SType = StructureType.DeviceQueueCreateInfo,
				QueueFamilyIndex = graphicsFamilyIndex,
				QueueCount = 1,
				PQueuePriorities = &priority
			};

			PhysicalDeviceFeatures features = new();

			DeviceCreateInfo deviceCreateInfo = new()
			{
				SType = StructureType.DeviceCreateInfo,

				PQueueCreateInfos = &queueCreateInfo,
				QueueCreateInfoCount = 1,

				PEnabledFeatures = &features
			};

			Vk.CreateDevice(physDevice, in deviceCreateInfo, null, out Device device).ThrowIfFailed();

			VulkanInfo = new()
			{
				Instance = instance,
				PhysicalDevice = physDevice,
				GraphicsQueueFamilyIndex = graphicsFamilyIndex,
				Device = device
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
		Vk.DestroyInstance(VulkanInfo.Value.Instance, null);
		Vk.Dispose();
	}
}
