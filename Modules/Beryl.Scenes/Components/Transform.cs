// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;

namespace Beryl.Scenes.Components;

/// <summary>
/// A 3D Position, Rotation and Scale.
/// </summary>
public class Transform : Component
{
	/// <summary> The position of the transform. </summary>
	public Vector3 Position { get; set; }

	/// <summary> The rotation of the transform. </summary>
	public Quaternion Rotation { get; set; }

	/// <summary> The scale of the transform. </summary>
	public Vector3 Scale { get; set; } = Vector3.One;

	/// <summary> The combined <see cref="Matrix4x4"/> of the transform. </summary>
	public Matrix4x4 Matrix => Matrix4x4.CreateTRS(Position, Rotation, Scale);

	/// <summary> The forward direction of the transform. </summary>
	public Vector3 Forward => Vector3.Transform(Vector3.Forward, Rotation);

	/// <summary> The right direction of the transform. </summary>
	public Vector3 Right => Vector3.Transform(Vector3.Right, Rotation);

	/// <summary> The up direction of the transform. </summary>
	public Vector3 Up => Vector3.Transform(Vector3.Up, Rotation);

	/// <summary> Creates a new instance of <see cref="Transform"/>. </summary>
	public Transform() { }

	/// <summary> Creates a new instance of <see cref="Transform"/> from the given position, rotation and scale. </summary>
	public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		Position = position;
		Rotation = rotation;
		Scale = scale;
	}
}
