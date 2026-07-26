// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Reflection;

using Beryl.Assemblies;

namespace Beryl.Serialization.Conversion;

/// <summary>
/// Marks a class as a string converter.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class StringConverterAttribute : Attribute
{
	private static Dictionary<Type, object>? converters = null;

	/// <summary> The type of the converter. </summary>
	public Type Type { get; }

	/// <summary> Initializes a new instance of <see cref="StringConverterAttribute"/>. </summary>
	public StringConverterAttribute(Type type) => Type = type;

	/// <summary> Gets the string converter for a type. </summary>
	public static object GetConverter(Type type)
	{
		if (converters is null)
		{
			converters = new Dictionary<Type, object>();

			foreach (var assembly in Reflection.GameAssemblies)
			{
				Type[] types = assembly.Types;

				foreach (var t in types)
				{
					var attr = t.GetCustomAttribute<StringConverterAttribute>();
					if (attr != null)
						converters[attr.Type] = t;
				}
			}
		}

		if (converters.TryGetValue(type, out object? converterType))
			return CreateInstance(converterType, type);

		if (type.IsEnum && converters.TryGetValue(typeof(Enum), out converterType))
			return CreateInstance(converterType, type);

		if (type.IsGenericType)
		{
			var openGeneric = type.GetGenericTypeDefinition();
			if (converters.TryGetValue(openGeneric, out converterType))
				return CreateInstance(converterType, type.GetGenericArguments()[0]);
		}

		var baseType = type.BaseType;
		while (baseType != null)
		{
			if (converters.TryGetValue(baseType, out converterType))
				return CreateInstance(converterType, type);
			baseType = baseType.BaseType;
		}

		throw new Exception($"No string converter found for type {type.Name}");
	}

	private static object CreateInstance(object converterTypeOrDefinition, Type targetType)
	{
		var t = (Type)converterTypeOrDefinition;

		if (t.IsGenericTypeDefinition)
			t = t.MakeGenericType(targetType);

		return Activator.CreateInstance(t)!;
	}
}

/// <summary>
/// A Class that converts an object to and from a string for serialization.
/// </summary>
internal abstract class StringConverter<T>
{
	/// <summary> Converts an object to a string. </summary>
	public abstract string ConvertToString(T value);

	/// <summary> Converts a string to an object. </summary>
	public abstract T ConvertFromString(string value);
}

[StringConverter(typeof(string))]
internal sealed class StringStringConverter : StringConverter<string>
{
	/// <inheritdoc/>
	public override string ConvertToString(string value) => value;

	/// <inheritdoc/>
	public override string ConvertFromString(string value) => value;
}
