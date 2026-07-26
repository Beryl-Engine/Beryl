// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Numerics;

using Beryl.Scenes.Entities;

namespace Beryl.Physics;

/// <summary>
/// Represents a simulated physics world.
/// </summary>
public interface IPhysicsWorld
{
	/// <summary> The amount of substeps to perform per step. A higher substep count results in a more accurate simulation, but is slower. </summary>
	public uint SubstepCount { get; set; }

	/// <summary> Creates a new physics body and adds it to the physics world. </summary>
	/// <returns> The created physics body. </returns>
	IPhysicsBody CreateBody(Entity? owner = null);

	/// <summary> Destroys a physics body and removes it from the physics world. </summary>
	/// <param name="body"> The physics body to remove. </param>
	void DestroyBody(IPhysicsBody body);

	/// <summary> Steps the physics simulation forward by a given time delta. </summary>
	/// <param name="delta"> The time in seconds to advance the simulation by. </param>
	void Step(float delta);

	/// <summary> Tries to perform a raycast in the physics world and returns if it hits something. </summary>
	/// <param name="origin"> The origin of the raycast. </param>
	/// <param name="direction"> The direction of the raycast. </param>
	/// <param name="maxDistance"> The maximum distance of the raycast. </param>
	/// <param name="result"> The result data of the raycast. </param>
	bool TryRaycast(Vector3 origin, Vector3 direction, float maxDistance, out RaycastResult result);
}
