// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Rendering.Pipelines.Forward;

public class ForwardRenderPipeline : RenderPipeline
{
	/// <inheritdoc/>
	public override RenderPass[] Passes { get; } = [new OpaquePass()];
}
