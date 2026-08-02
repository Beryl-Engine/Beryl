// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Reflection;
using System.Runtime.Loader;

namespace Beryl.Assemblies;

/// <summary>
/// Unloadable <see cref="System.Reflection.Assembly"/> with cached reflection.
/// </summary>
public sealed class BerylAssembly
{
	/// <summary> The path of the <see cref="BerylAssembly"/> </summary>
	public string Path { get; }

	/// <summary> The internal <see cref="System.Reflection.Assembly"/> instance loaded into memory. </summary>
	public Assembly Assembly { get; }

	/// <summary> The load context of the <see cref="BerylAssembly"/> or null if required assembly. </summary>
	internal AssemblyLoadContext? LoadContext { get; }

	/// <summary> Invoked when the <see cref="BerylAssembly"/> is <b>successfully</b> unloaded. </summary>
	public event Action<BerylAssembly> Unloaded = delegate { };

	/// <summary> All <see cref="Type"/>s in the <see cref="BerylAssembly"/>. </summary>
	/// <remarks> This is a cached property. </remarks>
	public Type[] Types
	{
		get
		{
			if (field is not null)
				return field;

			field = Assembly.GetTypes();
			return field;
		}
	}

	/// <summary> All static methods in the <see cref="BerylAssembly"/> marked with <see cref="OnAssemblyLoadAttribute"/>. </summary>
	/// <remarks> This is a cached property. </remarks>
	public Action[] AssemblyLoadDelegates
	{
		get
		{
			if (field is not null)
				return field;

			List<Action> delegates = new();

			foreach (Type type in Types)
			{
				foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (!method.IsDefined(typeof(OnAssemblyLoadAttribute), false))
						continue;

					if (method.GetParameters().Length != 0)
						continue;

					if (method.ReturnType != typeof(void))
						continue;

					Delegate del = method.CreateDelegate(typeof(Action));
					delegates.Add((Action)del);
				}
			}

			field = delegates.ToArray();
			return field;
		}
	}

	/// <summary> Initializes a new instance of <see cref="BerylAssembly"/>. </summary>
	internal BerylAssembly(string path, AssemblyLoadContext? loadContext, Assembly assembly)
	{
		Path = path;
		LoadContext = loadContext;
		Assembly = assembly;
	}

	/// <summary> Tries to unload the <see cref="BerylAssembly"/>. </summary>
	/// <returns> <see langword="true"/> if the <see cref="BerylAssembly"/> was unloaded, otherwise <see langword="false"/>. </returns>
	public bool TryUnload()
	{
		if (LoadContext is null || LoadContext.IsCollectible == false)
			return false;

		LoadContext.Unload();
		Unloaded(this);
		return true;
	}

	/// <inheritdoc/>
	public override string ToString() => $"{Path} ({LoadContext})";

	public static implicit operator Assembly(BerylAssembly asm) => asm.Assembly;
	public static implicit operator BerylAssembly(Assembly asm) => new(asm.Location, AssemblyLoadContext.GetLoadContext(asm), asm);
}
