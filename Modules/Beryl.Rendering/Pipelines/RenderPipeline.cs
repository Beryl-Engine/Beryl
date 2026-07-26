// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Rendering.Pipelines;

/// <summary>
/// A Collection of render passes.
/// </summary>
public abstract class RenderPipeline
{
	/// <summary> The render passes used by the pipeline. </summary>
	public abstract RenderPass[] Passes { get; }

	/// <summary> Renders the pipeline. </summary>
	public void Render(in RenderFrame frame)
	{
		foreach (var pass in Passes)
			pass.Execute(in frame);
	}
}
