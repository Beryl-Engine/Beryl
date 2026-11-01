// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;
using Beryl.RHI;

namespace Beryl.Rendering.Pipelines.Forward;

/// <summary>
/// Pass responsible for rendering all GUI.
/// </summary>
public class GUIPass : RenderPass
{
	/// <inheritdoc/>
	public override void Render(in RenderFrame frame, ICommandBuffer buffer)
	{
		foreach ((IClientRenderable Renderable, Matrix4x4 Transform) renderable in frame.Renderables)
		{
			if (renderable.Renderable.Material.Shader.Pass == "GUI")
				buffer.DrawImmediate(renderable.Renderable, renderable.Transform);
		}
	}
}
