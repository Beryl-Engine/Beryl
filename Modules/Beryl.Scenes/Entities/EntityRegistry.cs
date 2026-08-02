// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Reflection;

using Beryl.Assemblies;

namespace Beryl.Scenes.Entities;

/// <summary>
/// Handles the registration and management of <see cref="Entity"/>s.
/// </summary>
public static class EntityRegistry
{
	private static readonly Dictionary<string, Func<Entity>> factories = new();

	public static void RegisterFactories()
	{
		foreach (var assembly in Reflection.GameAssemblies)
		{
			var entities = assembly.Types.Where(t => typeof(Entity).IsAssignableFrom(t) && !t.IsAbstract);

			foreach (var entity in entities)
			{
				var attribute = entity.GetCustomAttribute<EntityAttribute>(false);
				if (attribute != null)
					RegisterFromType(attribute.Name ?? entity.Name, entity);
			}
		}
	}

	public static void Register<T>(string name) where T : Entity, new() => factories[name] = () => new T();

	private static void RegisterFromType(string name, Type type)
	{
		if (!typeof(Entity).IsAssignableFrom(type))
			return;

		if (type.IsAbstract)
			return;

		if (type.GetConstructor(Type.EmptyTypes) == null)
			return;

		factories[name] = () => (Entity)Activator.CreateInstance(type)!;
	}

	public static Entity CreateEntity(string name)
	{
		if (factories.TryGetValue(name, out var creator))
			return creator();

		throw new Exception($"No entity registered for '{name}'.");
	}

	public static T CreateEntity<T>(string name) where T : Entity => (T)CreateEntity(name);
}
