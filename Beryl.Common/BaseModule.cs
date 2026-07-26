// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Common;

public abstract class BaseModule : IDisposable
{
	/// <summary> Has this <see cref="BaseModule"/> been initialized? </summary>
	public bool IsInitialized { get; internal set; }

	/// <summary> Runs when <see cref="ModuleManager.InitializeAll"/> is called. </summary>
	public abstract void Initialize();

	/// <summary> Runs when <see cref="ModuleManager.DisposeAll"/> is called. </summary>
	/// <remarks> The default implementation unregisters this <see cref="BaseModule"/> from <see cref="ModuleManager"/>. </remarks>
	public virtual void Dispose()
	{
		ModuleManager.UnregisterModule(GetType());
		GC.SuppressFinalize(this);
	}
}
