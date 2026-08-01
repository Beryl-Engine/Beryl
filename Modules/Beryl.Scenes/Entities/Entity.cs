// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Reflection;
using System.Runtime.InteropServices;

using Beryl.Common;
using Beryl.Common.Utility;
using Beryl.Scenes.Components;
using Beryl.Serialization.Attributes;

namespace Beryl.Scenes.Entities;

/// <summary>
/// A Container for game logic and components.
/// </summary>
public class Entity
{
	/// <summary> Is this entity enabled? </summary>
	/// <remarks> Setting this will propogate the value down through all child <see cref="Component"/>s. </remarks>
	public bool Enabled
	{
		get;
		set
		{
			field = value;

			foreach (var component in CollectionsMarshal.AsSpan(components))
				component.Enabled = value;
		}
	} = true;

	/// <summary> The name of the entity. </summary>
	public string Name { get; set; } = "Entity";

	/// <summary> The <see cref="Components.Transform"/> used to derive entity position, rotation, and scale. </summary>
	public Transform Transform { get; set; } = new();

	/// <summary> The components attached to the entity. </summary>
	protected IReadOnlyList<Component> Components => components;

	[AlwaysSerialize]
	private readonly List<Component> components = new();

	/// <summary> Initializes a new instance of the <see cref="Entity"/> class. </summary>
	public Entity()
	{
		ModuleManager.GetModule<SceneModule>()?.CurrentScene.Entities.Add(this);

		foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
		{
			if (field.FieldType.IsAssignableTo(typeof(Component)) && field.GetCustomAttributes(typeof(ImplicitComponentAttribute), false).Length > 0)
			{
				var component = (Component?)field.GetValue(this);
				if (component == null)
				{
					component = (Component)Activator.CreateInstance(field.FieldType)!;
					field.SetValue(this, component);
				}

				AddComponent(component);
			}
		}

		foreach (var property in GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
		{
			if (property.PropertyType.IsAssignableTo(typeof(Component)) && property.GetCustomAttributes(typeof(ImplicitComponentAttribute), false).Length > 0)
			{
				var component = (Component?)property.GetValue(this);
				if (component == null)
				{
					var constructor = property.PropertyType.GetConstructor(Type.EmptyTypes);
					if (constructor == null)
					{
						BerylConsole.Warning($"Implicit component '{property.PropertyType.Name}' does not have a parameterless constructor, skipping.");
						continue;
					}

					component = (Component)Activator.CreateInstance(property.PropertyType)!;

					if (property.CanWrite)
					{
						property.SetValue(this, component);
					}
					else
					{
						string backingFieldName = $"<{property.Name}>k__BackingField";

						FieldInfo? backingField = GetType().GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
						backingField?.SetValue(this, component);
					}
				}

				AddComponent(component);
			}
		}
	}

	/// <summary> Called when the entity is enabled. </summary>
	/// <remarks> The base implementation starts all components. </remarks>
	public virtual void Start()
	{
		foreach (var component in CollectionsMarshal.AsSpan(components))
		{
			if (component.Enabled)
				component.Start();
		}
	}

	/// <summary> Called every frame. </summary>
	/// <remarks> The base implementation updates all components. </remarks>
	public virtual void Update()
	{
		foreach (var component in CollectionsMarshal.AsSpan(components))
		{
			if (component.Enabled)
				component.Update();
		}
	}

	/// <summary> Destroys the specified entity and releases any associated resources. </summary>
	public virtual void Destroy()
	{
		Enabled = false;

		foreach (var component in CollectionsMarshal.AsSpan(components))
		{
			component.Enabled = false;
			component.Destroy();
		}

		var scene = ModuleManager.GetModule<SceneModule>()?.CurrentScene;
		scene?.Entities.Remove(this);

		components.Clear();
		Transform = null!;
	}

	#region Component Methods
	/// <summary> Adds a component to the entity. </summary>
	protected Component AddComponent(Component component)
	{
		component.Entity = this;
		components.Add(component);

		return component;
	}

	/// <summary> Gets a component of type T from the entity. </summary>
	protected T? GetComponent<T>() where T : Component => (T?)components.Find(c => c is T);

	/// <summary> Adds a new component of type T to the entity. </summary>
	protected T AddComponent<T>() where T : Component, new() => (T)AddComponent(new T());

	/// <summary> Gets a component of type T from the entity or adds a new one if it doesn't exist. </summary>
	protected T GetOrAddComponent<T>() where T : Component, new() => GetComponent<T>() ?? AddComponent<T>();

	/// <summary> Removes a component from the entity. </summary>
	protected void RemoveComponent(Component component) => components.Remove(component);
	#endregion
}
