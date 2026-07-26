// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Utility;
using Beryl.Scenes.Entities;

namespace Beryl.Scenes.Resources;

/// <summary>
/// A collection of Entities.
/// </summary>
public class Scene
{
	/// <summary> All Entities in the Scene. </summary>
	public List<Entity> Entities { get; set; } = new();

	public void StartAll()
	{
		foreach (var entity in Entities)
		{
			if (!entity.Enabled)
				continue;

			try
			{
				entity.Start();
			}
			catch (Exception ex)
			{
				BerylConsole.Exception(ex);
			}
		}
	}

	internal void UpdateAll()
	{
		foreach (var entity in Entities)
		{
			if (!entity.Enabled)
				continue;

			try
			{
				entity.Update();
			}
			catch (Exception ex)
			{
				BerylConsole.Exception(ex);
			}
		}
	}
}
