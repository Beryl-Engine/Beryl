// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.Common.Utility;
using Beryl.Rendering.Resources;
using Beryl.Scenes.Components;

namespace Beryl.Rendering.Components;

public class MeshRenderer : Component
{
	/// <summary> The <see cref="Resources.Mesh"/> to render. </summary>
	public Mesh? Mesh { get; set; }

	public override void Start()
	{
		base.Start();

		if (Mesh == null)
			BerylConsole.Warning($"MeshRenderer '{Entity.Name}' has no mesh.");
	}

	/// <inheritdoc/>
	public override void Update()
	{
		base.Update();

		if (Mesh == null)
			return;

		ModuleManager.GetModule<RenderingModule>()?.QueueObject(Mesh, Entity.Transform.Matrix);
	}
}
