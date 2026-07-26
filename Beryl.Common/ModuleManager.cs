// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Reflection;

using Beryl.Common.Utility;

namespace Beryl.Common;

/// <summary>
/// Orchestrates all <see cref="BaseModule"/>s.
/// </summary>
public static class ModuleManager
{
	/// <summary> All registered <see cref="BaseModule"/>s. </summary>
	public static IReadOnlyList<BaseModule> Modules => modules.Values.ToList();

	private static readonly Dictionary<Type, BaseModule> modules = new();

	/// <summary> Registers a new <see cref="BaseModule"/> of type <typeparamref name="T"/>. If this is called after <see cref="InitializeAll"/> the <see cref="BaseModule"/> will need to manually call <see cref="BaseModule.Initialize"/>.</summary>
	/// <returns> The registered <see cref="BaseModule"/> instance. </returns>
	public static T RegisterModule<T>() where T : BaseModule
	{
		var module = Activator.CreateInstance<T>();
		modules.Add(typeof(T), module);
		return module;
	}

	/// <summary> Gets a registered <see cref="BaseModule"/> of type <typeparamref name="T"/>. </summary>
	/// <returns> The registered <see cref="BaseModule"/> instance or null if not found. </returns>
	public static T? GetModule<T>() where T : BaseModule => GetModule(typeof(T)) as T;

	/// <summary> Gets a registered <see cref="BaseModule"/> of type <paramref name="type"/>. </summary>
	/// <returns> The registered <see cref="BaseModule"/> instance or null if not found. </returns>
	public static BaseModule? GetModule(Type type)
	{
		if (modules.TryGetValue(type, out var module))
			return module;

		return null;
	}

	/// <summary> Unregisters a <see cref="BaseModule"/>. </summary>
	internal static void UnregisterModule(Type type) => modules.Remove(type);

	/// <summary> Initializes all <see cref="BaseModule"/>s currently registered. </summary>
	public static void InitializeAll()
	{
		foreach (var module in modules.Values)
			InitializeModule(module);
	}

	// Kinda hacky, we could maybe move to BaseModule.Initialize()?
	private static void InitializeModule(BaseModule module)
	{
		if (module.IsInitialized)
			return;

		var attr = module.GetType().GetCustomAttribute<InitializeAfterAttribute>();

		if (attr != null)
		{
			foreach (var dependencyType in attr.Types)
			{
				var dependency = GetModule(dependencyType);

				if (dependency == null)
				{
					BerylConsole.Warning($"Module {module.GetType().Name} depends on {dependencyType.Name} which is not registered.");
					return;
				}

				InitializeModule(dependency);
			}
		}

		module.Initialize();
		module.IsInitialized = true;

		BerylConsole.Log($"Initialized {module.GetType().Name}");
	}

	/// <summary> Disposes all <see cref="BaseModule"/>s currently registered. </summary>
	public static void DisposeAll()
	{
		foreach (var module in modules.Values)
		{
			module.IsInitialized = false;
			module.Dispose();

			BerylConsole.Log($"Disposed module {module.GetType().Name}");
		}
	}
}
