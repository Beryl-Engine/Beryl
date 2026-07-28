// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Scenes.Entities;
using Beryl.Serialization.Attributes;

namespace Beryl.Scenes.Components;

/// <summary>
/// A Container for reusable modular logic.
/// </summary>
public class Component
{
	/// <summary> Is this component enabled? </summary>
	public bool Enabled { get; set; } = true;

	/// <summary> The entity this component is attached to. </summary>
	[DontSerialize]
	public Entity Entity { get; internal set; } = null!;

	/// <summary> Called when the component is enabled. </summary>
	public virtual void Start() { }

	/// <summary> Called every frame. </summary>
	public virtual void Update() { }

	/// <summary> Called when <see cref="Entity"/> requests this component is destroyed. </summary>
	public virtual void Destroy() { }
}
