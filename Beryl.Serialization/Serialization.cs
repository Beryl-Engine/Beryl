// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Serialization.Backends;

namespace Beryl.Serialization;

public static class Serialization
{
	/// <summary> The default serialization type in use when another isn't specified. </summary>
	public const SerializationType DefaultType = SerializationType.YAML;

	/// <summary> Serializes an object to a byte array. </summary>
	public static byte[] Serialize<T>(T obj, SerializationType type = DefaultType)
	{
		if (obj == null)
			throw new ArgumentNullException(nameof(obj));

		InstanceRepresentation rep = DataConstructor.Construct(obj);

		Type? backendType = typeof(SerializerBackend).Assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(SerializerBackend)) && ((SerializerBackendAttribute)Attribute.GetCustomAttribute(t, typeof(SerializerBackendAttribute))!).SerializationType == type);
		if (backendType == null)
			throw new Exception($"No serializer backend found for type {type}.");

		var backend = (SerializerBackend)Activator.CreateInstance(backendType)!;
		return backend.Serialize(rep);
	}

	/// <summary> Deserializes a byte array to an object. </summary>
	/// <returns> The deserialized <typeparamref name="T"/> or <see langword="null"/> if deserialization failed. </returns>
	public static T? Deserialize<T>(byte[] data, SerializationType type = DefaultType) where T : new()
	{
		Type? backendType = typeof(SerializerBackend).Assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(SerializerBackend)) && ((SerializerBackendAttribute)Attribute.GetCustomAttribute(t, typeof(SerializerBackendAttribute))!).SerializationType == type);
		if (backendType == null)
			throw new Exception($"No serializer backend found for type {type}.");

		var backend = (SerializerBackend)Activator.CreateInstance(backendType)!;
		InstanceRepresentation rep = backend.Deserialize(data);
		return DataConstructor.Deconstruct<T>(rep);
	}
}
