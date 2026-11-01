// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.GUI.Widgets;
using Beryl.Math;

namespace Beryl.GUI.Layouts;

/// <summary>
/// Defines how one or more <see cref="Widget"/>s are positioned.
/// </summary>
public abstract class Layout
{
	private readonly List<Widget> pending = [];
	internal Widget? Owner { get; private set; }
	protected IReadOnlyList<Widget> Widgets => Owner?.Children ?? (IReadOnlyList<Widget>)pending;

	/// <summary> Adds a child to this layout's owner. </summary>
	public virtual void Add(Widget widget)
	{
		if (Owner != null)
			Owner.Children.Add(widget);
		else if (!pending.Contains(widget))
			pending.Add(widget);
	}

	public virtual void Remove(Widget widget)
	{
		if (Owner != null)
			Owner.Children.Remove(widget);
		else
			pending.Remove(widget);
	}

	internal void Attach(Widget owner)
	{
		if (Owner != null)
			throw new InvalidOperationException("A layout can only belong to one widget.");

		foreach (var child in pending)
		{
			if (!owner.Children.Contains(child))
				owner.Children.Validate(child);
		}

		Owner = owner;

		foreach (var child in pending)
		{
			if (!owner.Children.Contains(child))
				owner.Children.Add(child);
		}

		pending.Clear();
	}

	internal void Detach() => Owner = null;

	/// <summary> Measures the space requested by the children, excluding owner padding. </summary>
	public abstract Vector2 Measure(Vector2 availableSize);

	/// <summary> Arranges children within the supplied absolute logical content bounds. </summary>
	public abstract void UpdateGeometry(Rect rect);
}
