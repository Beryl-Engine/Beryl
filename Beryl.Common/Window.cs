// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;

using Silk.NET.Windowing;

namespace Beryl.Common;

/// <summary>
/// A Platform-agnostic Window.
/// </summary>
public class Window
{
	/// <summary> The internal Silk.NET window. </summary>
	public IWindow InternalWindow { get; }

	/// <summary> The size of the window. </summary>
	public Vector2I Size => new(InternalWindow.Size.X, InternalWindow.Size.Y);

	/// <summary> The position of the window. </summary>
	public Vector2I Position => new(InternalWindow.Position.X, InternalWindow.Position.Y);

	/// <summary> The current frame rate of the window. </summary>
	public int FramesPerSecond => Time.Delta > 0 ? (int)(1 / Time.Delta) : 0;

	/// <summary> The title of the window. </summary>
	public string Title { get => InternalWindow.Title; set => InternalWindow.Title = value; }

	/// <summary> Initializes a new instance of the <see cref="Window"/> class. </summary>
	public Window()
	{
		WindowOptions options = WindowOptions.Default with
		{
			API = GraphicsAPI.None
		};

		options.VSync = false;

		InternalWindow = Silk.NET.Windowing.Window.Create(options);
	}

	/// <summary> Starts the window loop. </summary>
	public void Run() => InternalWindow.Run();
}
