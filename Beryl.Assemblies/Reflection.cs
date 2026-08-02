// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Reflection;

namespace Beryl.Assemblies;

/// <summary>
/// Beryl-Optimized .NET Reflection.
/// </summary>
public static class Reflection
{
	/// <summary> All loaded <see cref="Assembly"/>s in the current <see cref="AppDomain"/> that belong to the engine (non-system). </summary>
	public static List<BerylAssembly> GameAssemblies { get; } = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.FullName?.Contains("System") ?? false).Select(x => (BerylAssembly)x).ToList();
}
