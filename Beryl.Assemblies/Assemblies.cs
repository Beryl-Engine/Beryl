// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Runtime.Loader;

namespace Beryl.Assemblies;

public static class Assemblies
{
	static Assemblies()
	{
		foreach (var loadedAssembly in Reflection.GameAssemblies)
			OnAssemblyLoadAttribute.RunOnAssembly(loadedAssembly);
	}

	/// <summary> Loads an assembly from the specified path. </summary>
	public static BerylAssembly LoadFromPath(string path)
	{
		string absolutePath = System.IO.Path.GetFullPath(path);

		var context = new AssemblyLoadContext("BerylPlugin", true);
		var assembly = context.LoadFromAssemblyPath(absolutePath);

		BerylAssembly asm = new(absolutePath, context, assembly);
		Reflection.GameAssemblies.Add(asm);
		asm.Unloaded += (a) => Reflection.GameAssemblies.Remove(a);

		OnAssemblyLoadAttribute.RunOnAssembly(assembly);

		return asm;
	}

	/// <summary> Unloads all currently loaded assemblies. </summary>
	public static void UnloadAll()
	{
		foreach (var asm in Reflection.GameAssemblies)
			asm.TryUnload();
	}
}
