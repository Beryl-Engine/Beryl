// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Runtime.InteropServices;

using Beryl.Common.Utility;
using Beryl.Rendering;
using Beryl.RHI;

using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.OpenXR;

namespace Beryl.VirtualReality.OpenXR.Bindings;

/// <summary>
/// Managed bindings for <see cref="Instance"/>.
/// </summary>
internal sealed class OpenXRInstance : OpenXRBinding
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private unsafe delegate Result pfnGetD3D12GraphicsRequirementsKHR(Instance instance, ulong systemId, GraphicsRequirementsD3D12KHR* req);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private unsafe delegate Result pfnGetVulkanGraphicsRequirementsKHR(Instance instance, ulong systemId, GraphicsRequirementsVulkanKHR* req);

	/// <summary> The native <see cref="Instance"/>. </summary>
	public Instance Native { get; }

	/// <summary> The list of extensions that this instance supports. </summary>
	public IReadOnlyList<string> Extensions => extensions;

	/// <summary> The name of the Virtual Reality headset. </summary>
	public string DeviceName { get; private set; }

	/// <summary> The ID of the current system. </summary>
	public ulong SystemID
	{
		get
		{
			ulong systemID = 0;

			SystemGetInfo systemInfo = new()
			{
				Type = StructureType.SystemGetInfo,
				FormFactor = FormFactor.HeadMountedDisplay
			};

			Result res = OpenXR.GetSystem(Native, ref systemInfo, ref systemID);
			if (res != Result.Success)
			{
				systemInfo.FormFactor = FormFactor.HandheldDisplay;
				OpenXR.GetSystem(Native, ref systemInfo, ref systemID).VerifySuccess();
			}

			return systemID;
		}
	}

	private readonly List<string> extensions = new();

	public OpenXRInstance(XR openXR) : base(openXR)
	{
		extensions = GetRequiredExtensions() ?? new List<string>();
		IntPtr extensionsPtr = SilkMarshal.StringArrayToPtr((IReadOnlyList<string>)Extensions);

		var appInfo = new ApplicationInfo()
		{
			ApiVersion = new Version64(1, 0, 0)
		};

		unsafe
		{
			var appNameSpan = new Span<byte>(appInfo.ApplicationName, 128);
			var engineNameSpan = new Span<byte>(appInfo.EngineName, 128);

			SilkMarshal.StringIntoSpan("Beryl", appNameSpan);
			SilkMarshal.StringIntoSpan("None", engineNameSpan);

			var createInfo = new InstanceCreateInfo
			{
				Type = StructureType.InstanceCreateInfo,
				ApplicationInfo = appInfo,
				EnabledExtensionCount = (uint)extensions.Count,
				EnabledExtensionNames = (byte**)extensionsPtr,
				Next = null
			};

			Instance instance = new();
			OpenXR.CreateInstance(ref createInfo, ref instance).VerifySuccess();

			Native = instance;

			SystemProperties properties = new()
			{
				Type = StructureType.SystemProperties
			};

			OpenXR.GetSystemProperties(instance, SystemID, ref properties).VerifySuccess();

			DeviceName = Marshal.PtrToStringAnsi((IntPtr)properties.SystemName) ?? "Unknown";

			CheckGraphicsRequirements();
		}
	}

	private unsafe List<string>? GetRequiredExtensions()
	{
		List<string> extensions = new();

		string requiredExtension = Renderer.Backend switch
		{
			RendererBackend.Vulkan => "XR_KHR_vulkan_enable",
			RendererBackend.Direct3D12 => "XR_KHR_D3D12_enable",
			_ => throw new NotSupportedException("Unsupported renderer backend for OpenXR")
		};

		extensions.Add(requiredExtension);

		uint propCount = 0;
		OpenXR.EnumerateInstanceExtensionProperties((byte*)null, 0, &propCount, null).VerifySuccess();

		Span<ExtensionProperties> props = new ExtensionProperties[propCount];
		for (int i = 0; i < props.Length; i++)
		{
			props[i].Type = StructureType.ExtensionProperties;
			props[i].Next = null;
		}

		OpenXR.EnumerateInstanceExtensionProperties((byte*)null, propCount, &propCount, props).VerifySuccess();

		bool found = false;
		for (int i = 0; i < props.Length; i++)
		{
			fixed (void* namePtr = props[i].ExtensionName)
			{
				string name = SilkMarshal.PtrToString((IntPtr)namePtr)!;
				if (name == requiredExtension)
				{
					found = true;
					break;
				}
			}
		}

		if (!found)
		{
			BerylConsole.Warning($"Required OpenXR extension not supported: {requiredExtension}");
			return null;
		}

		return extensions;
	}

	// This needs to run for a valid instance
	private unsafe void CheckGraphicsRequirements()
	{
		switch (Renderer.Backend)
		{
			case RendererBackend.Direct3D12:
				{
					PfnVoidFunction fnPtr = new();
					OpenXR.GetInstanceProcAddr(Native, "xrGetD3D12GraphicsRequirementsKHR", ref fnPtr).VerifySuccess();

					var fn = Marshal.GetDelegateForFunctionPointer<pfnGetD3D12GraphicsRequirementsKHR>((IntPtr)fnPtr.Handle);

					GraphicsRequirementsD3D12KHR requirements = new()
					{
						Type = StructureType.GraphicsRequirementsD3D12Khr
					};

					fn(Native, SystemID, &requirements);

					break;
				}

			case RendererBackend.Vulkan:
				{
					PfnVoidFunction fnPtr = new();
					OpenXR.GetInstanceProcAddr(Native, "xrGetVulkanGraphicsRequirementsKHR", ref fnPtr).VerifySuccess();

					var fn = Marshal.GetDelegateForFunctionPointer<pfnGetVulkanGraphicsRequirementsKHR>((IntPtr)fnPtr.Handle);

					GraphicsRequirementsVulkanKHR requirements = new()
					{
						Type = StructureType.GraphicsRequirementsVulkanKhr
					};

					fn(Native, SystemID, &requirements);

					break;
				}

			default:
				throw new NotSupportedException("Unsupported renderer backend for OpenXR");
		}
	}
}
