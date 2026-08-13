// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI.VulkanBackend;

namespace Beryl.RHI;

/// <summary>
/// Creates graphics devices for the configured rendering backend.
/// </summary>
public static class GraphicsDeviceFactory
{
	/// <summary> Determines whether <paramref name="backend"/> is supported. </summary>
	public static bool IsBackendSupported(RendererBackend backend) => backend switch
	{
		RendererBackend.Vulkan => true, // TODO
		RendererBackend.Direct3D12 => true,
		_ => false
	};

	/// <summary> Creates a graphics device for <paramref name="backend"/>. </summary>
	public static IGraphicsDevice Create(RendererBackend backend) => backend switch
	{
		RendererBackend.Vulkan => new VulkanDevice(),
		RendererBackend.Direct3D12 => throw new NotImplementedException(),
		_ => throw new NotSupportedException($"Backend {backend} is not supported.")
	};
}
