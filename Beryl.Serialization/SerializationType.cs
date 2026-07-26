// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Serialization;

/// <summary>
/// The serialization backend type.
/// </summary>
public enum SerializationType : byte
{
	YAML,
	JSON,
	Binary
}
