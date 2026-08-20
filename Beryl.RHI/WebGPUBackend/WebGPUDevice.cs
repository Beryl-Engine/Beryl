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
	/// <inheritdoc/>
	public VulkanInfo? VulkanInfo { get; private set; }

	/// <inheritdoc/>
	public DeviceFeatures Features { get; } = DeviceFeatures.None;

	/// <inheritdoc/>
	public IFramebuffer SwapchainFramebuffer => throw new NotImplementedException();

	/// <inheritdoc/>
	public IResourceFactory ResourceFactory { get; }

	private Surface? surface;
	private Instance? instance;
	private Device? device;

	private readonly RendererBackend backend;

	public WebGPUDevice(RendererBackend backend)
	{
		ResourceFactory = new WebGPUResourceFactory(this);
		this.backend = backend;
	}

	/// <inheritdoc/>
	public void Initialize(IWindow window)
	{
		instance = WebGPU.CreateInstance();
		if (instance == null)
			throw new Exception("Failed to create WebGPU instance.");

		surface = CreateSurface(window.Native, instance);
		if (surface == null)
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
			CompatibleSurface = surface,
			PowerPreference = PowerPreference.HighPerformance,

			BackendType = backendType
		};

		instance.RequestAdapter(in adapterOptions, (adapterStatus, adapter, bytes) =>
		{
			if (adapterStatus != RequestAdapterStatus.Success || adapter == null)
				throw new Exception("Failed to create WebGPU adapter.");

			var info = adapter.GetInfo();

			BerylConsole.Log($"Found GPU - {info.Device}");

			adapter.RequestDevice((deviceStatus, device, bytes) =>
			{
				if (deviceStatus != RequestDeviceStatus.Success || device == null)
					throw new Exception("Failed to create WebGPU device.");

				this.device = device;

				surface.Configure(new SurfaceConfiguration()
				{
					Device = this.device,
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
		if (surface == null || device == null)
			return;

		surface.Configure(new SurfaceConfiguration()
		{
			Device = device,
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
	public void SwapBuffers() => surface?.Present();

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
