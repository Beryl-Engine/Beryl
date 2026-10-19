// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.RHI;

/// <summary>
/// Creates graphics devices for the configured rendering backend.
/// </summary>
public static class GraphicsDeviceFactory
{
	/// <summary> Checks platform, runtime and driver support for <paramref name="backend"/>. Window presentation is validated during device initialization. </summary>
	public static bool IsBackendSupported(RendererBackend backend)
	{
		try
		{
			return backend switch
			{
				RendererBackend.Vulkan => (OperatingSystem.IsWindows() || OperatingSystem.IsLinux()) && GraphicsBackendSupport.IsVulkanSupported(),
				RendererBackend.Direct3D12 => OperatingSystem.IsWindowsVersionAtLeast(10) && GraphicsBackendSupport.IsDirect3D12Supported(),
				_ => false
			};
		}
		catch (Exception exception) when (exception is DllNotFoundException or EntryPointNotFoundException or BadImageFormatException or TypeInitializationException or SharpGen.Runtime.SharpGenException)
		{
			return false;
		}
	}

	/// <summary> Creates a graphics device for <paramref name="backend"/>. </summary>
	public static IGraphicsDevice Create(RendererBackend backend) => new WebGPUBackend.WebGPUDevice(backend);
}
