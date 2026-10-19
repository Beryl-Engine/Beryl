// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.RHI;

/// <summary>
/// Creates graphics devices for the configured rendering backend.
/// </summary>
public static class GraphicsDeviceFactory
{
	/// <summary> Determines whether <paramref name="backend"/> is supported. </summary>
	public static bool IsBackendSupported(RendererBackend backend) => WebGPUBackend.WebGPUDevice.IsBackendSupported(backend);

	/// <summary> Creates a graphics device for <paramref name="backend"/>. </summary>
	public static IGraphicsDevice Create(RendererBackend backend) => new WebGPUBackend.WebGPUDevice(backend);
}
