// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Utility;

namespace Beryl.Common.Resources.DefaultProvider;

/// <summary>
/// Base class for handling of resource loading.
/// </summary>
public abstract class ResourceLoader<T> where T : class
{
	/// <summary> Loads a resource from the given byte data. </summary>
	/// <param name="data"> The byte data. </param>
	/// <param name="name"> The name of the resource or null if not applicable. </param>
	/// <returns> The loaded resource or null if invalid. </returns>
	public abstract T? LoadResource(byte[] data, string? name = null);

	/// <summary> Gets an instance of the resource loader this type. </summary>
	/// <returns> The resource loader for the specified type or null if not found. </returns>
	public static ResourceLoader<T>? GetResourceLoader()
	{
		var loaderType = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.FirstOrDefault(t =>
				typeof(ResourceLoader<T>).IsAssignableFrom(t) &&
				t.GetCustomAttributes(typeof(ResourceLoaderAttribute), inherit: false).Any());

		if (loaderType == null)
		{
			BerylConsole.Warning($"No resource loader found for type '{typeof(T).Name}'.");
			return null;
		}

		return (ResourceLoader<T>)Activator.CreateInstance(loaderType)!;
	}
}

/// <summary>
/// Marks a class as a resource loader.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ResourceLoaderAttribute : Attribute { }
