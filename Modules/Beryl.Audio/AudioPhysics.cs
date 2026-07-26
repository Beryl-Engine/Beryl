// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Audio.Components;
using Beryl.Common;
using Beryl.Math;
using Beryl.Physics;
using Beryl.Scenes.Resources;

namespace Beryl.Audio;

/// <summary>
/// Responsible for processing physics-based audio propagation and obstructions.
/// </summary>
public class AudioPhysics
{
	private PhysicsModule? physicsModule;

	/// <summary> Updates the audio physics state for all entities in the active scene. </summary>
	/// <param name="activeScene"> The scene to process. </param>
	public void Update(Scene activeScene)
	{
		physicsModule ??= ModuleManager.GetModule<PhysicsModule>();
		if (physicsModule?.World == null)
			return;

		AudioListener? listener = FindActiveListener(activeScene);
		if (listener == null)
			return;

		ProcessAudioObstructions(activeScene, listener, physicsModule.World);
	}

	private AudioListener? FindActiveListener(Scene scene)
	{
		foreach (var entity in scene.Entities)
		{
			if (!entity.Enabled)
				continue;

			var listener = entity.GetComponent<AudioListener>();
			if (listener != null)
				return listener;
		}

		return null;
	}

	private void ProcessAudioObstructions(Scene scene, AudioListener listener, IPhysicsWorld physicsWorld)
	{
		Vector3 listenerPos = listener.Entity.Transform.Position;

		foreach (var entity in scene.Entities)
		{
			if (!entity.Enabled)
				continue;

			var source = entity.GetComponent<AudioSource>();
			if (source == null)
				continue;

			source.Obstructed = EvaluateObstruction(source, listenerPos, physicsWorld);
		}
	}

	private bool EvaluateObstruction(AudioSource source, Vector3 listenerPos, IPhysicsWorld physicsWorld)
	{
		Vector3 sourcePos = source.Entity.Transform.Position;
		Vector3 direction = listenerPos - sourcePos;
		float distance = direction.Length;

		if (distance <= 0.001f)
			return false;

		Vector3 normalizedDir = direction / distance;

		return physicsWorld.TryRaycast(sourcePos, normalizedDir, distance, out _);
	}
}
