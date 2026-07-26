// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;

namespace Beryl.GUI.Widgets;

/// <summary>
/// Base class for all GUI widgets.
/// </summary>
public class Widget
{
	public Rect Rect { get; set; }

	public virtual void Update()
	{

	}

	public virtual void Draw(PaintEngine pEngine) => pEngine.DrawRect(Rect, Color.White);
}
