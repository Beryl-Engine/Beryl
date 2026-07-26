// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Scenes.Entities;

/// <summary>
/// Registers a class inherting from <see cref="Entity"/> into the Entity system.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EntityAttribute : Attribute
{
	/// <summary> The text name of the entity. (i.e. "my_awesome_entity") </summary>
	public string Name { get; }

	/// <summary> Initializes a new instance of the <see cref="EntityAttribute"/> class. </summary>
	public EntityAttribute(string name)
	{
		Name = name;
	}
}


/// <summary>
/// Indicates that the property or field of the entity is an implicit component.
/// </summary>
/// <remarks> An implicit component is a component that is automatically added to the <see cref="Entity"/> upon creation. </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ImplicitComponentAttribute : Attribute { }
