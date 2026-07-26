// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Serialization.Attributes;

/// <summary>
/// Forces a property to never serialize even if it would otherwise be serialized.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DontSerializeAttribute : Attribute { }
