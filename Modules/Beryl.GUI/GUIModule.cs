// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.GUI.Widgets;
using Beryl.Math;

namespace Beryl.GUI;

/// <summary>
/// Module responsible for GUI.
/// </summary>
public class GUIModule : BaseModule
{
	public PaintEngine PaintEngine { get; set; } = new();

	public Widget RootWidget { get; set; } = new Widget() { Rect = new Rect(0, 0, 50, 50) };

	/// <inheritdoc/>
	public override void Initialize() => Application.OnUpdate += Update;

	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();

		Application.OnUpdate -= Update;
	}

	public void Update()
	{
		RootWidget.Update();
		RootWidget.Draw(PaintEngine);
	}
}
