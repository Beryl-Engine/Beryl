// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Datatypes;

namespace Beryl.Rendering;

public sealed class GPUBufferPool : ObjectPool<GPUBufferPool, GPUBuffer>
{
	/// <inheritdoc/>
	protected override int MaxPoolSize => 2048;

	/// <inheritdoc/>
	protected override GPUBuffer CreateObject() => new();

	/// <inheritdoc/>
	protected override void ResetObject(GPUBuffer obj)
	{
		base.ResetObject(obj);

		obj.Reset();
	}
}
