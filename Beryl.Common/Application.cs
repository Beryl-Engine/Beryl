// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Resources;
using Beryl.Common.Resources.DefaultProvider;
using Beryl.Common.Utility;
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

	/// <summary> Information and settings for the application. </summary>
	public static ProjectInfo Info { get; private set; } = null!;

	/// <summary> The resource provider. </summary>
	public static IResourceProvider ResourceProvider { get; set; } = null!;

	/// <summary> The main window. </summary>
	public static Window Window { get; private set; } = null!;

	public static void Initialize(ProjectInfo? projectInfo = null, IResourceProvider? resourceProvider = null)
	{
		resourceProvider ??= new DefaultResourceProvider();

		ResourceProvider = resourceProvider;

		projectInfo ??= ResourceProvider.GetResource<ProjectInfo>("ProjectInfo.beryl");
		if (projectInfo == null)
		{
			BerylConsole.Warning($"No {nameof(ProjectInfo)} passed to {nameof(Application)}.{nameof(Initialize)} and can't find '{nameof(ProjectInfo)}.beryl' in project root, using defaults.");
			projectInfo = new ProjectInfo();
		}

		Info = projectInfo;

		Window = new Window()
		{
			Title = Info.Name
		};

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
