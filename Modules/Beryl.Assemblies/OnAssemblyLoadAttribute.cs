// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Assemblies;

/// <summary>
/// Marks a static method to be ran automatically when the assembly is loaded.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class OnAssemblyLoadAttribute : Attribute
{
	public static void RunOnAssembly(BerylAssembly assembly)
	{
		foreach (Action del in assembly.AssemblyLoadDelegates)
			del();
	}
}
