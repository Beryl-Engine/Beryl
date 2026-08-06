// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Runtime.CompilerServices;
using Beryl.Common;
using Beryl.Rendering;
using Beryl.RHI;
using Silk.NET.Core.Native;
using Silk.NET.OpenXR;

namespace Beryl.VirtualReality.OpenXR.Bindings;

/// <summary>
/// Managed bindings for <see cref="GraphicsBinding"/>.
/// </summary>
internal sealed class OpenXRGraphicsBinding : OpenXRBinding
{
	/// <summary> Pointer to the native graphics binding. </summary>
	public IntPtr Native { get; }

	// We use these as lifetime storage
	private GraphicsBindingVulkanKHR vulkanBinding;
	private GraphicsBindingD3D12KHR d3d12Binding;

	public OpenXRGraphicsBinding(XR openXR, RendererBackend target) : base(openXR)
	{
		var rendering = ModuleManager.GetModule<RenderingModule>()!;

		unsafe
		{
			switch (target)
			{
				case RendererBackend.Vulkan:
					if (rendering.Device.VulkanInfo == null)
						throw new Exception();

					vulkanBinding = new GraphicsBindingVulkanKHR
					{
						Type = StructureType.GraphicsBindingVulkanKhr,

						Instance = new VkHandle(rendering.Device.VulkanInfo.Value.Instance),
						PhysicalDevice = new VkHandle(rendering.Device.VulkanInfo.Value.PhysicalDevice),
						Device = new VkHandle(rendering.Device.VulkanInfo.Value.Device),

						QueueFamilyIndex = rendering.Device.VulkanInfo.Value.GraphicsQueueFamilyIndex,
						QueueIndex = 0
					};
					Native = (IntPtr)Unsafe.AsPointer(ref vulkanBinding);
					break;

				case RendererBackend.Direct3D12:
					d3d12Binding = new GraphicsBindingD3D12KHR()
					{
						Type = StructureType.GraphicsBindingD3D12Khr,
						// Device = (void*)rendering.Device.GraphicsDevice.GetD3D12Info().Device,
					};
					Native = (IntPtr)Unsafe.AsPointer(ref d3d12Binding);
					break;

				default:
					break;

			}
		}
	}
}
