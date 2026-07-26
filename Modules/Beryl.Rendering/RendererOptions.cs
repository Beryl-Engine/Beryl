// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.RHI;

namespace Beryl.Rendering;

public struct RendererOptions
{
	/// <summary> The culling mode to use. </summary>
	public CullingMode Culling { get; set; }
}
