// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Serialization.Backends;

/// <summary>
/// Marks a class as a serializer backend.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class SerializerBackendAttribute : Attribute
{
	/// <summary> The serialization type this backend is for. </summary>
	public SerializationType SerializationType { get; }

	/// <summary> Initializes a new instance of the <see cref="SerializerBackendAttribute"/> class. </summary>
	public SerializerBackendAttribute(SerializationType serializationType) => SerializationType = serializationType;
}

internal abstract class SerializerBackend
{
	/// <summary> Serializes the specified object. </summary>
	public abstract byte[] Serialize(InstanceRepresentation obj);

	/// <summary> Deserializes the specified data into an <see cref="InstanceRepresentation"/>. </summary>
	public abstract InstanceRepresentation Deserialize(byte[] data);
}
