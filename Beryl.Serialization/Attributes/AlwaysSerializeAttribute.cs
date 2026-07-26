// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Serialization.Attributes;

/// <summary>
/// Forces a property or field to be serialized when it typically wouldn't be.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class AlwaysSerializeAttribute : Attribute { }
