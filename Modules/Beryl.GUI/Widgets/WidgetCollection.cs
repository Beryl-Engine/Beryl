// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Collections.ObjectModel;

namespace Beryl.GUI.Widgets;

public sealed class WidgetCollection(Widget owner) : Collection<Widget>, IReadOnlyList<Widget>
{
	internal void Validate(Widget child)
	{
		ArgumentNullException.ThrowIfNull(child);
		if (child.Parent != null)
			throw new InvalidOperationException("Remove the widget from its current parent before adding it.");

		for (Widget? ancestor = owner; ancestor != null; ancestor = ancestor.Parent)
		{
			if (ReferenceEquals(ancestor, child))
				throw new InvalidOperationException("A widget cannot contain itself or an ancestor.");
		}
	}

	protected override void InsertItem(int index, Widget item)
	{
		Validate(item);
		base.InsertItem(index, item);
		item.Parent = owner;
	}

	protected override void SetItem(int index, Widget item)
	{
		if (ReferenceEquals(this[index], item))
			return;

		Validate(item);
		var old = this[index];
		base.SetItem(index, item);
		old.Parent = null;
		item.Parent = owner;
	}

	protected override void RemoveItem(int index)
	{
		var old = this[index];
		base.RemoveItem(index);
		old.Parent = null;
	}

	protected override void ClearItems()
	{
		foreach (var child in this)
			child.Parent = null;

		base.ClearItems();
	}
}
