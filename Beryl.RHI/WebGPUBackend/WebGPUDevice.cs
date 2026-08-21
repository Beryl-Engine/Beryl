// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Utility;
using Beryl.RHI.Resources;
using Silk.NET.Core.Contexts;
using Silk.NET.Windowing;
using WebGpuSharp;
using WebGpuSharp.FFI;

namespace Beryl.RHI.WebGPUBackend;

internal sealed class WebGPUDevice : IGraphicsDevice
{
	internal Surface Surface { get; private set; } = null!;
	internal Instance Instance { get; private set; } = null!;
	internal Device Device { get; private set; } = null!;

	/// <inheritdoc/>
	public VulkanInfo? VulkanInfo { get; private set; }

	/// <inheritdoc/>
	public DeviceFeatures Features { get; } = DeviceFeatures.None;

	/// <inheritdoc/>
	public IFramebuffer SwapchainFramebuffer => throw new NotImplementedException();

	/// <inheritdoc/>
	public IResourceFactory ResourceFactory { get; }

	private readonly RendererBackend backend;

	public WebGPUDevice(RendererBackend backend)
	{
		ResourceFactory = new WebGPUResourceFactory(this);
		this.backend = backend;
	}

	/// <inheritdoc/>
	public void Initialize(IWindow window)
	{
		Instance = WebGPU.CreateInstance();
		if (Instance == null)
			throw new Exception("Failed to create WebGPU instance.");

		Surface = CreateSurface(window.Native, Instance);
		if (Surface == null)
			throw new Exception("Failed to create surface for window.");

		BackendType backendType = BackendType.Undefined;
		switch (backend)
		{
			case RendererBackend.Vulkan:
				backendType = BackendType.Vulkan;
				break;
			case RendererBackend.Direct3D12:
				backendType = BackendType.D3D12;
				break;
		}

		RequestAdapterOptions adapterOptions = new()
		{
			CompatibleSurface = Surface,
			PowerPreference = PowerPreference.HighPerformance,

			BackendType = backendType
		};

		Instance.RequestAdapter(in adapterOptions, (adapterStatus, adapter, bytes) =>
		{
			if (adapterStatus != RequestAdapterStatus.Success || adapter == null)
				throw new Exception("Failed to create WebGPU adapter.");

			var info = adapter.GetInfo();

			BerylConsole.Log($"Found GPU - {info.Device}");

			adapter.RequestDevice((deviceStatus, device, bytes) =>
			{
				if (deviceStatus != RequestDeviceStatus.Success || device == null)
					throw new Exception("Failed to create WebGPU device.");

				Device = device;

				Surface.Configure(new SurfaceConfiguration()
				{
					Device = Device,
					Format = WebGpuSharp.TextureFormat.BGRA8Unorm,
					Width = (uint)window.Size.X,
					Height = (uint)window.Size.Y,
					Usage = TextureUsage.RenderAttachment,
					PresentMode = PresentMode.Immediate
				});
			});
		});
	}

	/// <inheritdoc/>
	public void ResizeSwapchain(uint width, uint height)
	{
		if (Surface == null || Device == null)
			return;

		Surface.Configure(new SurfaceConfiguration()
		{
			Device = Device,
			Format = WebGpuSharp.TextureFormat.BGRA8Unorm,
			Width = width,
			Height = height,
			Usage = TextureUsage.RenderAttachment,
			PresentMode = PresentMode.Immediate
		});
	}

	/// <inheritdoc/>
	public void SubmitCommands(ICommandBuffer buffer) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SwapBuffers() => Surface?.Present();

	/// <inheritdoc/>
	public void UpdateBuffer<T>(IBuffer buffer, uint offset, ReadOnlySpan<T> data) where T : unmanaged => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Dispose()
	{

	}

	private static unsafe Surface? CreateSurface(INativeWindow? native, Instance instance)
	{
		ArgumentNullException.ThrowIfNull(native);

		if (native.Win32 is { } win32)
		{
			var winDesc = new SurfaceSourceWindowsHWNDFFI
			{
				Chain = new ChainedStruct {  SType = SType.SurfaceSourceWindowsHWND },
				Hinstance = (void*)win32.HInstance,
				Hwnd = (void*)win32.Hwnd
			};

			return instance.CreateSurface(new SurfaceDescriptor(ref winDesc));
		}

		if (native.X11 is { } x11)
		{
			var x11Desc = new SurfaceSourceXlibWindowFFI
			{
				Chain = new ChainedStruct { SType = SType.SurfaceSourceXlibWindow },
				Display = (void*)x11.Display,
				Window = x11.Window
			};

			return instance.CreateSurface(new SurfaceDescriptor(ref x11Desc));
		}

		if (native.Wayland is { } wayland)
		{
			var waylandDesc = new SurfaceSourceWaylandSurfaceFFI
			{
				Chain = new ChainedStruct { SType = SType.SurfaceSourceWaylandSurface },
				Display = (void*)wayland.Display,
				Surface = (void*)wayland.Surface
			};

			return instance.CreateSurface(new SurfaceDescriptor(ref waylandDesc));
		}

		return null;
	}
}
