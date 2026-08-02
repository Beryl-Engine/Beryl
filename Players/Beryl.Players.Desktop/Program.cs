// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Assemblies;
using Beryl.Audio;
using Beryl.Common;
using Beryl.GUI;
using Beryl.Input;
using Beryl.Math;
using Beryl.Physics;
using Beryl.Physics.Components.Colliders;
using Beryl.Rendering;
using Beryl.Rendering.Entities;
using Beryl.Rendering.Resources;
using Beryl.Scenes;

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
		ModuleManager.RegisterModule<RenderingModule>();

		Application.OnStart += () =>
		{
			Skybox sky = new();

			// Setup debug scene
			FlyController flyController = new();

			Prop floor = new() { PhysicsMode = PropPhysicsMode.Static };
			floor.Transform.Scale = new Vector3(10, 1, 10);
			floor.Renderer.Mesh = Mesh.Plane;

			floor.RigidBody.Colliders.Add(new BoxCollider(10, 0.01f, 10));

			Prop cube = new() { PhysicsMode = PropPhysicsMode.Dynamic };
			cube.Transform.Position = new Vector3(0, 10f, 0);
			cube.Renderer.Mesh = Mesh.Cube;

			cube.RigidBody.Colliders.Add(new BoxCollider(1, 1, 1));

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
