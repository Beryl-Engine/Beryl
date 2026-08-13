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

			InstanceCreateInfo createInfo = new()
			{
				SType = StructureType.InstanceCreateInfo,

				PApplicationInfo = &appInfo,
				
				EnabledExtensionCount = count,
				PpEnabledExtensionNames = windowExtensions,
				EnabledLayerCount = 0,
			};

			Vk.CreateInstance(in createInfo, null, out Instance instance).ThrowIfFailed();

			PhysicalDevice physDevice = Vk.GetOptimalDevice(instance);
			var properties = Vk.GetPhysicalDeviceProperty(physDevice);

			string? deviceName = SilkMarshal.PtrToString((IntPtr)properties.DeviceName, NativeStringEncoding.UTF8);
			BerylConsole.Log($"Using Optimal GPU: {deviceName}");

			VulkanInfo = new()
			{
				Instance = instance,
				PhysicalDevice = physDevice
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
	public void Dispose() => throw new NotImplementedException();
}
