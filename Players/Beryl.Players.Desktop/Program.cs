// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Assemblies;
using Beryl.Audio;
using Beryl.Common;
using Beryl.GUI;
using Beryl.Input;
using Beryl.Math;
using Beryl.Physics;
using Beryl.Rendering;
using Beryl.Rendering.Components;
using Beryl.Rendering.Entities;
using Beryl.Rendering.Resources;
using Beryl.Scenes;
using Beryl.Scenes.Entities;
using Beryl.VirtualReality;

namespace Beryl.Desktop;

internal sealed class Program
{
	static void Main(string[] args)
	{
		// Register all Modules our game wants to use
		ModuleManager.RegisterModule<InputModule>();
		ModuleManager.RegisterModule<AssemblyModule>();
		ModuleManager.RegisterModule<PhysicsModule>();
		ModuleManager.RegisterModule<SceneModule>();
		ModuleManager.RegisterModule<AudioModule>();
		ModuleManager.RegisterModule<GUIModule>();
		ModuleManager.RegisterModule<VirtualRealityModule>();
		ModuleManager.RegisterModule<RenderingModule>();

		Application.OnStart += () =>
		{
			// Setup debug scene
			FlyController flyController = new();
			flyController.Name = "Camera";

			DebugEntity floor = new();
			floor.Name = "Floor";
			floor.Transform.Scale = new Vector3(10, 1, 10);

			Mesh? cubeMesh = Application.ResourceProvider.GetResource<Mesh>("Assets/PrimitiveCube.fbx");
			Shader? shader = Application.ResourceProvider.GetResource<Shader>("Assets/Beryl/Unlit.slang");
			cubeMesh?.Material = new(shader ?? throw new Exception("Shader not found"));

			Entity cube = new();
			cube.Transform.Position = new Vector3(0, 2, 0);
			MeshRenderer renderer = cube.AddComponent<MeshRenderer>();
			renderer.Mesh = cubeMesh;

			ModuleManager.GetModule<SceneModule>()?.CurrentScene.StartAll();
		};

		Application.OnExit += () =>
		{

		};

		Application.OnUpdate += () =>
		{

		};

		Application.OnRender += () =>
		{

		};

		Application.Initialize();
	}
}
