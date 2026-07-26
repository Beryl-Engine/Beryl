// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Utility;
using Beryl.Math;
using Beryl.RHI.Resources;
using Beryl.Rendering.Resources;
using Beryl.RHI;

namespace Beryl.Rendering;

/// <summary>
/// Buffer that assembles data in a format readable by an <see cref="IGraphicsDevice"/>.
/// </summary>
public sealed class GPUBuffer
{
	public enum AlignmentFormat : byte
	{
		/// <summary> Rigid layout with strict 16-byte alignment and padding. </summary>
		std140,

		/// <summary> Tightly packed layout using natural data type sizes. </summary>
		std430,
	}

	/// <summary> The initial size of the internal buffer. </summary>
	public const int DEFAULT_SIZE = 256;

	/// <summary> Padded data of the buffer. </summary>
	public ReadOnlySpan<byte> Data
	{
		get
		{
			int globalAlignment = Alignment == AlignmentFormat.std140 ? 16 : 4;
			int paddedSize = Align(offset, globalAlignment);

			if (paddedSize > offset)
			{
				EnsureCapacity(paddedSize);
				data.AsSpan(offset, paddedSize - offset).Clear();
			}

			return data.AsSpan(0, paddedSize);
		}
	}

	/// <summary> The <see cref="AlignmentFormat"/> used to construct <see cref="Data"/>. </summary>
	/// <remarks> This should be set before adding data to the buffer. </remarks>
	public AlignmentFormat Alignment { get; set; }

	private byte[] data = new byte[DEFAULT_SIZE];
	private int offset = 0;

	/// <summary> Adds the parameters of a <see cref="Material"/> to the buffer. </summary>
	public void AddMaterialParameters(Material mat)
	{
		ReadOnlySpan<ShaderParameter> paramSpan = mat.Shader.Resources.First(x => x.Name == "Parameters").Parameters.AsSpan();

		foreach (ref readonly var param in paramSpan)
		{
			switch (param)
			{
				case { Type: ShaderParameter.ParamType.Float, DefaultValue: float f }:
					AddFloat(f);
					break;

				case { Type: ShaderParameter.ParamType.Int, DefaultValue: long i }:
					AddInt((int)i);
					break;

				case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector3 v }:
					AddFloat3(v.X, v.Y, v.Z);
					break;

				case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector2 v }:
					AddFloat2(v.X, v.Y);
					break;

				case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector4 v }:
					AddFloat4(v.X, v.Y, v.Z, v.W);
					break;

				case { Type: ShaderParameter.ParamType.SampledTexture2D, DefaultValue: Texture text }:
					// TODO
					break;

				case { DefaultValue: null }:
					BerylConsole.Warning($"Parameter has no default value: {param.Name} ({param.Type})");
					break;

				default:
					BerylConsole.Warning($"Unsupported GPUBuffer solver for parameter: {param.Name} ({param.Type})");
					break;
			}
		}
	}

	public void AddInt(int value)
	{
		EnsurePacking(4, 4);
		EnsureCapacity(offset + 4);

		Span<byte> dest = data.AsSpan(offset, 4);
		BitConverter.TryWriteBytes(dest, value);

		offset += 4;
	}

	public void AddFloat(float value)
	{
		EnsurePacking(4, 4);
		EnsureCapacity(offset + 4);

		Span<byte> dest = data.AsSpan(offset, 4);
		BitConverter.TryWriteBytes(dest, value);

		offset += 4;
	}

	public void AddFloat2(float x, float y)
	{
		EnsurePacking(8, 8);
		EnsureCapacity(offset + 8);
		BitConverter.TryWriteBytes(data.AsSpan(offset, 4), x);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 4, 4), y);
		offset += 8;
	}

	public void AddFloat3(float x, float y, float z)
	{
		EnsurePacking(12, 16);
		EnsureCapacity(offset + 12);
		BitConverter.TryWriteBytes(data.AsSpan(offset, 4), x);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 4, 4), y);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 8, 4), z);
		offset += 12;
	}

	public void AddFloat4(float x, float y, float z, float w)
	{
		EnsurePacking(16, 16);
		EnsureCapacity(offset + 16);
		BitConverter.TryWriteBytes(data.AsSpan(offset, 4), x);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 4, 4), y);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 8, 4), z);
		BitConverter.TryWriteBytes(data.AsSpan(offset + 12, 4), w);
		offset += 16;
	}

	public void AddMatrix4x4(Matrix4x4 m)
	{
		AddFloat4(m.M11, m.M12, m.M13, m.M14);
		AddFloat4(m.M21, m.M22, m.M23, m.M24);
		AddFloat4(m.M31, m.M32, m.M33, m.M34);
		AddFloat4(m.M41, m.M42, m.M43, m.M44);
	}

	/// <summary> Adds raw bytes directly to the buffer (e.g. for structs via MemoryMarshal). </summary>
	public void AddBytes(ReadOnlySpan<byte> bytes)
	{
		EnsureCapacity(offset + bytes.Length);
		bytes.CopyTo(data.AsSpan(offset));
		offset += bytes.Length;
	}

	/// <summary> Resets the buffer to it's default state. </summary>
	public void Reset()
	{
		offset = 0;
		Alignment = default;
	}

	private void EnsurePacking(int size, int baseAlign)
	{
		int aligned = Align(offset, baseAlign);

		if (Alignment == AlignmentFormat.std140 && (aligned % 16) + size > 16)
			aligned = Align(aligned, 16);

		EnsureCapacity(aligned);
		while (offset < aligned)
		{
			data[offset] = 0;
			offset++;
		}
	}

	private void EnsureCapacity(int required)
	{
		if (required <= data.Length)
			return;

		int newSize = data.Length;
		while (newSize < required)
			newSize *= 2;

		Array.Resize(ref data, newSize);
	}

	private static int Align(int offset, int alignment) => (offset + alignment - 1) / alignment * alignment;
}
