// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.Resources;
using Silk.NET.Windowing;

namespace Beryl.RHI.VulkanBackend;

internal sealed class VulkanDevice : IGraphicsDevice
{
	/// <inheritdoc/>
	public VulkanInfo? VulkanInfo { get; } = new();

	/// <inheritdoc/>
	public DeviceFeatures Features { get; } = DeviceFeatures.ClipSpaceYInverted;

	/// <inheritdoc/>
	public IFramebuffer SwapchainFramebuffer => throw new NotImplementedException();

	/// <inheritdoc/>
	public IResourceFactory ResourceFactory => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Initialize(IWindow window)
	{

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
