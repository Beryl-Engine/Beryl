// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Common.Resources;

/// <summary>
/// A Provider for game resources/assets.
/// </summary>
public interface IResourceProvider
{
	/// <summary> Gets a resource from its path. </summary>
	/// <returns> The resource or null if invalid. </returns>
	T? GetResource<T>(string path) where T : class;
}
