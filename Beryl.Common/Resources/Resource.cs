// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Common.Resources;

/// <summary>
/// Base class for all game resources loadable via <see cref="Application.ResourceProvider"/>.
/// </summary>
public abstract class Resource
{
	/// <summary> Unique Identifier for this <see cref="Resource"/>. </summary>
	/// <remarks> This is a stable reference per session only. </remarks>
	public Guid ID { get; protected set; } = Guid.NewGuid();
}
