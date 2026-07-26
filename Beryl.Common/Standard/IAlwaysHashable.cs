// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Common.Standard;

/// <summary>
/// Defines a type that always has an implementation of <see cref="GetHashCode"/> or <see cref="object.GetHashCode"/>.
/// </summary>
/// <remarks>
/// This interface is useful for implementing non-boxing hash functions. For instance, it is the only type constraint that can be used to get the hash code of a <c>ref struct</c> without throwing <see cref="object"/> boxing errors.
/// </remarks>
public interface IAlwaysHashable
{
	int GetHashCode();
}

