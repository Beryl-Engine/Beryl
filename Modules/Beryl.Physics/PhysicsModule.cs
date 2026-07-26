// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.Physics.Engines.Jitter2;

namespace Beryl.Physics;

/// <summary>
/// Module responsible for handling the physics simulation.
/// </summary>
public class PhysicsModule : BaseModule
{
	public IPhysicsWorld World { get; set; } = null!;

	private double accumulator = 0f;

	/// <inheritdoc/>
	public override void Initialize()
	{
		Application.OnUpdate += Update;

		World = new Jitter2World();
	}

	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();

		Application.OnUpdate -= Update;
	}

	/// <inheritdoc/>
	public void Update()
	{
		accumulator += Time.PreciseDelta;
		while (accumulator > Time.FixedDelta)
		{
			accumulator -= Time.FixedDelta;
			World.Step(Time.FixedDelta);
		}
	}
}
