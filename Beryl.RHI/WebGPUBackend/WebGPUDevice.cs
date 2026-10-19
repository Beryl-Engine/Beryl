// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Runtime.InteropServices;
using Beryl.Common.Utility;
using Beryl.RHI.Resources;
using Silk.NET.Core.Contexts;
using Silk.NET.Windowing;
using WebGpuSharp;
using WebGpuSharp.FFI;
using WebGpuSharp.Marshalling;

namespace Beryl.RHI.WebGPUBackend;

internal sealed class WebGPUDevice : IGraphicsDevice
{
	internal Surface Surface { get; private set; } = null!;
	internal Instance Instance { get; private set; } = null!;
	internal Device Device { get; private set; } = null!;
	internal Queue Queue { get; private set; } = null!;
	internal TextureView DepthView { get; private set; } = null!;
	internal WebGpuSharp.TextureFormat SurfaceFormat { get; private set; }

	/// <inheritdoc/>
	public VulkanInfo? VulkanInfo => null;

	/// <inheritdoc/>
	public DeviceFeatures Features => DeviceFeatures.None;

	/// <inheritdoc/>
	public IFramebuffer SwapchainFramebuffer { get; }

	/// <inheritdoc/>
	public IResourceFactory ResourceFactory { get; }

	private readonly RendererBackend backend;
	private Texture? depthTexture;
	private Texture? currentTexture;
	private TextureView? currentView;
	private PresentMode presentMode;
	private uint width;
	private uint height;
	private bool disposed;

	public WebGPUDevice(RendererBackend backend)
	{
		this.backend = backend;
		ResourceFactory = new WebGPUResourceFactory(this);
		SwapchainFramebuffer = new WebGPUFramebuffer(this);
	}

	internal static bool IsBackendSupported(RendererBackend backend)
	{
		if (backend is not (RendererBackend.Vulkan or RendererBackend.Direct3D12))
			return false;

		try
		{
			var instance = WebGPU.CreateInstance();
			return instance?.RequestAdapterSync(new RequestAdapterOptions
			{
				CompatibleSurface = null!,
				BackendType = backend == RendererBackend.Vulkan ? BackendType.Vulkan : BackendType.D3D12
			}) != null;
		}
		catch (Exception)
		{
			return false;
		}
	}

	/// <inheritdoc/>
	public void Initialize(IWindow window)
	{
		ObjectDisposedException.ThrowIf(disposed, this);
		if (Instance != null)
			throw new InvalidOperationException("The WebGPU device is already initialized.");

		try
		{
			Instance = WebGPU.CreateInstance() ?? throw new InvalidOperationException("Failed to create WebGPU instance.");
			Surface = CreateSurface(window.Native, Instance) ?? throw new PlatformNotSupportedException("No supported native window handle was available.");

			var adapter = Instance.RequestAdapterSync(new RequestAdapterOptions
			{
				CompatibleSurface = Surface,
				PowerPreference = PowerPreference.HighPerformance,
				BackendType = backend switch
				{
					RendererBackend.Vulkan => BackendType.Vulkan,
					RendererBackend.Direct3D12 => BackendType.D3D12,
					_ => throw new ArgumentOutOfRangeException(nameof(backend))
				}
			});

			BerylConsole.Log($"Found GPU - {adapter.GetInfo().Device}");

			Device = adapter.RequestDeviceSync();
			Queue = Device.GetQueue();

			var capabilities = Surface.GetCapabilities(adapter) ?? throw new InvalidOperationException("Failed to query surface capabilities.");
			if (capabilities.Formats.IsEmpty)
				throw new InvalidOperationException("The surface has no supported texture formats.");

			SurfaceFormat = capabilities.Formats.Contains(WebGpuSharp.TextureFormat.BGRA8Unorm) ? WebGpuSharp.TextureFormat.BGRA8Unorm : capabilities.Formats[0];
			presentMode = !window.VSync && capabilities.PresentModes.Contains(PresentMode.Immediate) ? PresentMode.Immediate : PresentMode.Fifo;

			ResizeSwapchain((uint)System.Math.Max(1, window.FramebufferSize.X), (uint)System.Math.Max(1, window.FramebufferSize.Y));
		}
		catch
		{
			Dispose();
			throw;
		}
	}

	/// <inheritdoc/>
	public void ResizeSwapchain(uint width, uint height)
	{
		ObjectDisposedException.ThrowIf(disposed, this);
		if (Surface == null || Device == null || width == 0 || height == 0)
			return;

		ReleaseCurrentTexture();

		depthTexture?.Destroy();
		this.width = width;
		this.height = height;

		ConfigureSurface();

		depthTexture = Device.CreateTexture(new WebGpuSharp.TextureDescriptor
		{
			Size = new(width, height, 1),
			Dimension = TextureDimension.D2,
			Format = WebGpuSharp.TextureFormat.Depth32Float,
			Usage = TextureUsage.RenderAttachment
		});

		DepthView = depthTexture.CreateView();
	}

	private void ConfigureSurface() => Surface.Configure(new SurfaceConfiguration
	{
		Device = Device,
		Format = SurfaceFormat,
		Width = width,
		Height = height,
		Usage = TextureUsage.RenderAttachment,
		PresentMode = presentMode,
		AlphaMode = CompositeAlphaMode.Auto
	});

	internal TextureView GetCurrentView()
	{
		ObjectDisposedException.ThrowIf(disposed, this);
		if (currentView != null)
			return currentView;

		var surfaceTexture = Surface.GetCurrentTexture();
		if (surfaceTexture.Status is SurfaceGetCurrentTextureStatus.Outdated or SurfaceGetCurrentTextureStatus.Lost)
		{
			ConfigureSurface();
			surfaceTexture = Surface.GetCurrentTexture();
		}

		if (surfaceTexture.Status is not (SurfaceGetCurrentTextureStatus.SuccessOptimal or SurfaceGetCurrentTextureStatus.SuccessSuboptimal) || surfaceTexture.Texture == null)
			throw new InvalidOperationException($"Failed to acquire a WebGPU surface texture: {surfaceTexture.Status}.");

		currentTexture = surfaceTexture.Texture;
		currentView = currentTexture.CreateView();
		return currentView;
	}

	internal void ReleaseCurrentTexture()
	{
		currentView = null;
		currentTexture = null;
	}

	/// <inheritdoc/>
	public void SubmitCommands(ICommandBuffer buffer) => ((WebGPUCommandBuffer)buffer).Submit();

	/// <inheritdoc/>
	public void SwapBuffers()
	{
		if (currentTexture == null)
			return;

		Surface.Present();
		ReleaseCurrentTexture();
	}

	/// <inheritdoc/>
	public void UpdateBuffer<T>(IBuffer buffer, uint offset, ReadOnlySpan<T> data) where T : unmanaged
	{
		var target = (WebGPUBuffer)buffer;
		ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes(data);
		ValidateBufferUpdate(target, offset, bytes.Length);
		if (!bytes.IsEmpty)
			Queue.WriteBuffer(target.Resource, offset, bytes);
	}

	internal static void ValidateBufferUpdate(WebGPUBuffer buffer, uint offset, int length)
	{
		if ((offset & 3) != 0 || (length & 3) != 0)
			throw new ArgumentException("WebGPU buffer updates require four-byte aligned offsets and lengths.");

		if ((ulong)offset + (ulong)length > buffer.SizeInBytes)
			throw new ArgumentOutOfRangeException(nameof(offset), "The update exceeds the buffer size.");
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		if (disposed)
			return;

		disposed = true;
		ReleaseCurrentTexture();
		depthTexture?.Destroy();
		depthTexture = null;
		DepthView = null!;

		if (Surface != null && width != 0)
			Surface.Unconfigure();

		if (Device != null)
			WebGPUMarshal.GetHandle(Device).Destroy();

		Queue = null!;
		Device = null!;
		Surface = null!;
		Instance = null!;
	}

	private static unsafe Surface? CreateSurface(INativeWindow? native, Instance instance)
	{
		ArgumentNullException.ThrowIfNull(native);

		if (native.Win32 is { } win32)
		{
			var winDesc = new SurfaceSourceWindowsHWNDFFI
			{
				Chain = new ChainedStruct { SType = SType.SurfaceSourceWindowsHWND },
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
