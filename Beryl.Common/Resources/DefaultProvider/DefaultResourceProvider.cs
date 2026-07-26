// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Utility;

namespace Beryl.Common.Resources.DefaultProvider;

internal sealed class DefaultResourceProvider : IResourceProvider
{
	/// <inheritdoc/>
	public T? GetResource<T>(string path) where T : class
	{
		if (!File.Exists(path))
		{
			BerylConsole.Warning($"Requested resource '{path}' does not exist.");
			return null;
		}

		byte[] data = File.ReadAllBytes(path);
		return ResourceLoader<T>.GetResourceLoader()?.LoadResource(data, Path.GetFileNameWithoutExtension(path));
	}
}
