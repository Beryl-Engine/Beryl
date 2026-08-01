// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;

namespace Beryl.Rendering.Resources;

/// <summary>
/// Represents the visual appearance of a mesh by using a <see cref="Resources.Shader"/>
/// and its associated parameters, such as textures, colors, and numerical values.
/// </summary>
public class Material
{
	/// <summary> The GPU <see cref="Resources.Shader"/> used by the material. </summary>
	public Shader Shader { get; set; }

	/// <summary> A default material using a simple shader. </summary>
	public static Material Default { get; }

	private readonly Dictionary<string, object> properties = new();

	/// <summary> Initializes a new <see cref="Material"/> from the specified <see cref="Resources.Shader"/>. </summary>
	public Material(Shader shader)
	{
		Shader = shader;
	}

	static Material()
	{
		Shader? def = Application.ResourceProvider.GetResource<Shader>("Assets/Defaults/DummyLit.slang");
		if (def == null)
		{
			Default = new Material(new Shader(""));
			return;
		}

		Default = new Material(def);
	}

	/// <summary> Sets the value of a property in the material's shader. </summary>
	public void SetProperty<T>(string name, T value)
	{
		if (value is null)
			throw new ArgumentNullException(nameof(value));

		properties[name] = value;
	}

	/// <summary> Gets the value of a property from the material's shader. </summary>
	public T GetProperty<T>(string name)
	{
		object? value = properties[name];

		if (value is T tValue)
			return tValue;

		throw new InvalidCastException($"Cannot cast parameter '{name}' to type {typeof(T).Name}");
	}
}
