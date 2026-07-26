// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Rendering;

using Beryl.Common.Datatypes;
using Beryl.RHI;

public class CommandBufferPool : ObjectPool<CommandBufferPool, ICommandBuffer>
{
	/// <inheritdoc/>
	protected override ICommandBuffer CreateObject() => Renderer.Device.ResourceFactory.CreateCommandBuffer();
}
