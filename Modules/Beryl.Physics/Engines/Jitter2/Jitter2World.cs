// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Numerics;

using Jitter2;
using Jitter2.Collision;
using Jitter2.Collision.Shapes;
using Jitter2.Dynamics;
using Jitter2.LinearMath;
using Beryl.Scenes.Entities;

namespace Beryl.Physics.Engines.Jitter2;

/// <summary>
/// Represents a Jitter2 physics world.
/// </summary>
public class Jitter2World : IPhysicsWorld
{
	/// <inheritdoc/>
	public uint SubstepCount { get => (uint)world.SubstepCount; set => world.SubstepCount = (int)value; }

	private readonly World world;
	private readonly Dictionary<RigidBody, Jitter2Body> bodyMap = new();

	/// <summary> Creates a new Jitter2 physics world. </summary>
	public Jitter2World()
	{
		world = new World();
		world.SubstepCount = (int)SubstepCount;
	}

	/// <inheritdoc/>
	public IPhysicsBody CreateBody(Entity? owner = null)
	{
		var rb = world.CreateRigidBody();
		var jb = new Jitter2Body(rb, bodyMap)
		{
			Owner = owner
		};

		bodyMap[rb] = jb;
		return jb;
	}

	/// <inheritdoc/>
	public void DestroyBody(IPhysicsBody body)
	{
		if (body is Jitter2Body b && b.body != null)
			world.Remove(b.body);
	}

	/// <inheritdoc/>
	public void Step(float delta) => world.Step(delta, true);

	/// <inheritdoc/>
	public bool TryRaycast(Vector3 origin, Vector3 direction, float maxDistance, out RaycastResult hit)
	{
		var jOrigin = new JVector(origin.X, origin.Y, origin.Z);
		var jDirection = new JVector(direction.X, direction.Y, direction.Z);

		float dirLength = jDirection.Length();
		if (dirLength < 1e-6f)
		{
			hit = default;
			return false;
		}

		var jDirectionNorm = JVector.Multiply(jDirection, 1f / dirLength);

		bool didHit = world.DynamicTree.RayCast(
			jOrigin,
			jDirectionNorm,
			maxDistance,
			pre: null,
			post: null,
			out IDynamicTreeProxy? proxy,
			out JVector hitNormal,
			out float lambda
		);

		if (!didHit || proxy is not RigidBodyShape shape || shape.RigidBody == null)
		{
			hit = default;
			return false;
		}

		if (!bodyMap.TryGetValue(shape.RigidBody, out var jitterBody))
		{
			hit = default;
			return false;
		}

		hit = new RaycastResult
		{
			Body = jitterBody,
			HitPoint = origin + direction * (float)lambda,
			Normal = new Vector3((float)hitNormal.X, (float)hitNormal.Y, (float)hitNormal.Z),
			Distance = (float)lambda
		};

		return true;
	}
}
