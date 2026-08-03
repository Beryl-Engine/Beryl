// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Reflection;

using Beryl.Serialization.Attributes;
using Beryl.Serialization.Conversion;

namespace Beryl.Serialization;

/// <summary>
/// Class responsible for converting an instance to a <see cref="InstanceRepresentation"/>.
/// </summary>
internal static class DataConstructor
{
	/// <summary> Constructs a <see cref="InstanceRepresentation"/> from an instance. </summary>
	public static InstanceRepresentation Construct<T>(T instance) => new() { Fields = ConstructFields(instance!, null, new HashSet<object>(ReferenceEqualityComparer.Instance)) };

	/// <summary> Deconstructs a <see cref="InstanceRepresentation"/> to an instance. </summary>
	public static T? Deconstruct<T>(InstanceRepresentation rep) => (T?)DeconstructObject(typeof(T), rep.Fields);

	private static FieldRepresentation[] ConstructFields(object instance, string? prefix, HashSet<object> visited)
	{
		// Structs are value types, only track reference types
		if (!instance.GetType().IsValueType)
		{
			if (!visited.Add(instance))
				return Array.Empty<FieldRepresentation>();
		}

		var type = instance.GetType();
		var fields = new List<FieldRepresentation>();

		// Properties
		foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttribute<DontSerializeAttribute>() == null && p.GetIndexParameters().Length == 0 && (p.CanWrite && p.CanRead || p.GetCustomAttribute<AlwaysSerializeAttribute>() != null)))
		{
			object? value = prop.GetValue(instance);
			string fullName = prefix == null ? prop.Name : $"{prefix}.{prop.Name}";
			fields.AddRange(SerializeValue(prop.PropertyType, value, fullName, visited));
		}

		// Fields
		foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.GetCustomAttribute<AlwaysSerializeAttribute>() != null && f.GetCustomAttribute<DontSerializeAttribute>() == null))
		{
			object? value = field.GetValue(instance);
			string fullName = prefix == null ? field.Name : $"{prefix}.{field.Name}";
			fields.AddRange(SerializeValue(field.FieldType, value, fullName, visited));
		}

		return fields.ToArray();
	}

	private static bool TryConvertToString(Type type, object value, out string? result)
	{
		try
		{
			object converter = StringConverterAttribute.GetConverter(type);
			var method = converter.GetType().GetMethod("ConvertToString")!;
			result = (string)method.Invoke(converter, new[] { value })!;
			return true;
		}
		catch { result = null; return false; }
	}

	private static object? DeconstructObject(Type type, FieldRepresentation[] fields)
	{
		object? instance = Activator.CreateInstance(type);

		var flat = fields.Where(f => !f.Name.Contains('.')).ToArray();
		var nested = fields.Where(f => f.Name.Contains('.')).GroupBy(f => f.Name.Split('.')[0]).ToDictionary(g => g.Key, g => g.Select(f => new FieldRepresentation(f.Name[(f.Name.IndexOf('.') + 1)..], f.Value)).ToArray());

		// Properties
		foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttribute<DontSerializeAttribute>() == null && p.CanWrite))
			DeconstructMember(prop.Name, prop.PropertyType, v => prop.SetValue(instance, v), flat, nested);

		// Fields
		foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.GetCustomAttribute<AlwaysSerializeAttribute>() != null && f.GetCustomAttribute<DontSerializeAttribute>() == null))
			DeconstructMember(field.Name, field.FieldType, v => field.SetValue(instance, v), flat, nested);

		return instance;
	}

	private static IEnumerable<FieldRepresentation> SerializeValue(Type type, object? value, string fullName, HashSet<object> visited)
	{
		if (value == null)
		{
			yield return new FieldRepresentation(fullName, "");
			yield break;
		}

		if (TryConvertToString(type, value, out string? stringValue))
		{
			yield return new FieldRepresentation(fullName, stringValue!);
		}
		else if (IsNestedObject(type))
		{
			foreach (var f in ConstructFields(value, fullName, visited))
				yield return f;
		}
		else
		{
			throw new Exception($"No string converter found for type {type.Name}.");
		}
	}

	private static void DeconstructMember(string name, Type memberType, Action<object?> setValue, FieldRepresentation[] flat, Dictionary<string, FieldRepresentation[]> nested)
	{
		if (flat.FirstOrDefault(f => f.Name == name) is { Name: not null } field)
		{
			if (string.IsNullOrEmpty(field.Value))
				return;

			try
			{
				object converter = StringConverterAttribute.GetConverter(memberType);
				var method = converter.GetType().GetMethod("ConvertFromString")!;
				setValue(method.Invoke(converter, new object[] { field.Value }));
			}
			catch
			{
				throw new Exception($"No string converter found for type {memberType.Name}.");
			}
		}
		else if (nested.TryGetValue(name, out var childFields))
		{
			setValue(DeconstructObject(memberType, childFields));
		}
	}

	private static bool IsNestedObject(Type type) => !type.IsPrimitive && type != typeof(string) && !type.IsEnum && (type.GetProperties().Length > 0 || type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Any(f => f.GetCustomAttribute<AlwaysSerializeAttribute>() != null));
}
