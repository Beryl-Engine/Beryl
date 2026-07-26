// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Common;

/// <summary>
/// Marks a class inheriting from <see cref="BaseModule"/> to be initialized after the given <see cref="BaseModule"/> types have been initialized.
/// </summary>
/// <remarks>
/// <see cref="BaseModule"/> types passed to this attribute will only initialize first if they have been registered with <see cref="ModuleManager.RegisterModule{T}"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class InitializeAfterAttribute : Attribute
{
	/// <summary> The <see cref="BaseModule"/> types to initialize after. </summary>
	public Type[] Types { get; }

	public InitializeAfterAttribute(params Type[] types) => Types = types;
}
