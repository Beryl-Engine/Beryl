// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Silk.NET.OpenXR;

namespace Beryl.VirtualReality.OpenXR.Bindings;

/// <summary>
/// Base class for all OpenXR managed bindings.
/// </summary>
internal class OpenXRBinding
{
	public XR OpenXR { get; }

	public OpenXRBinding(XR openXR)
	{
		OpenXR = openXR;
	}
}
