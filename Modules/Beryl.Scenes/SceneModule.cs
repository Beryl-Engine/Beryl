// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.Scenes.Entities;
using Beryl.Scenes.Resources;

namespace Beryl.Scenes;

/// <summary>
/// Module responsible for managing scenes.
/// </summary>
public class SceneModule : BaseModule
{
	/// <summary> The currently loaded scene. </summary>
	public Scene CurrentScene { get; set; } = null!;

	/// <inheritdoc/>
	public override void Initialize()
	{
		Application.OnUpdate += Update;

		EntityRegistry.RegisterFactories();

		CurrentScene = new Scene();
	}

	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();

		Application.OnUpdate -= Update;
	}

	public void Update() => CurrentScene.UpdateAll();
}
