// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Resources;
using Beryl.Common.Resources.DefaultProvider;
using Beryl.Math;

namespace Beryl.Common;

/// <summary>
/// The top-level application.
/// </summary>
public static class Application
{
	/// <summary> Called when the application starts. </summary>
	public static event Action? OnStart;

	/// <summary> Called when the application exits. </summary>
	public static event Action? OnExit;

	/// <summary> Called when the application updates. </summary>
	public static event Action? OnUpdate;

	/// <summary> Called when the application renders. </summary>
	public static event Action? OnRender;

	/// <summary> Called when the application resizes. </summary>
	public static event Action<Vector2>? OnResize;

	/// <summary> The resource provider. </summary>
	public static IResourceProvider ResourceProvider { get; set; } = null!;

	/// <summary> The main window. </summary>
	public static Window Window { get; private set; } = null!;

	public static void Initialize(IResourceProvider? resourceProvider = null)
	{
		if (resourceProvider == null)
			resourceProvider = new DefaultResourceProvider();

		ResourceProvider = resourceProvider;

		Window = new Window();

		Window.InternalWindow.Load += () => { ModuleManager.InitializeAll(); OnStart?.Invoke(); };
		Window.InternalWindow.Closing += () => { ModuleManager.DisposeAll(); OnExit?.Invoke(); };
		Window.InternalWindow.Render += (delta) => OnRender?.Invoke();
		Window.InternalWindow.Resize += (size) => OnResize?.Invoke(new Vector2(size.X, size.Y));
		Window.InternalWindow.Update += (delta) =>
		{
			Time.Delta = (float)delta;
			Time.PreciseDelta = delta;

			OnUpdate?.Invoke();
		};

		Window.Run();
	}
}
