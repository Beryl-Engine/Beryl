// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;
using Silk.NET.Windowing;

namespace Beryl.RHI;

/// <summary>
/// Low-Level interface into the current Graphics API.
/// </summary>
public interface IGraphicsDevice : IDisposable
{
	#region Native
	/// <summary> Low-Level information about the Vulkan backend or null if not using Vulkan. </summary>
	VulkanInfo? VulkanInfo { get; }
	#endregion

	/// <summary> Unique features of this <see cref="IGraphicsDevice"/> implementation. </summary>
	DeviceFeatures Features { get; }

	/// <summary> The <see cref="IFramebuffer"/> used by the main swapchain. </summary>
	IFramebuffer SwapchainFramebuffer { get; }

	/// <summary> The <see cref="IResourceFactory"/> used to create GPU resources. </summary>
	IResourceFactory ResourceFactory { get; }

	/// <summary> Initializes the <see cref="IGraphicsDevice"/> for <paramref name="window"/>. </summary>
	void Initialize(IWindow window);

	/// <summary> Submits <paramref name="buffer"/> for execution. </summary>
	void SubmitCommands(ICommandBuffer buffer);

	/// <summary> Swaps the front and back buffers. </summary>
	void SwapBuffers();

	/// <summary> Resizes the swapchain. </summary>
	void ResizeSwapchain(uint width, uint height);

	/// <summary> Updates <paramref name="buffer"/> with unmanaged <paramref name="data"/>. </summary>
	void UpdateBuffer<T>(IBuffer buffer, uint offset, ReadOnlySpan<T> data) where T : unmanaged;
}
