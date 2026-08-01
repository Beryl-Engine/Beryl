// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Utility;
using Beryl.Math;
using Beryl.RHI.Resources;
using Beryl.Rendering.Resources;
using Beryl.RHI;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

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
				case { Type: ShaderParameter.ParamType.Float, DefaultValue: ShaderValue.Float f }:
					AddFloat(f.Value);
					break;

				case { Type: ShaderParameter.ParamType.Int, DefaultValue: ShaderValue.Int i }:
					AddInt(i.Value);
					break;

				case { Type: ShaderParameter.ParamType.Vector, DefaultValue: ShaderValue.Vector v }:
					AddSpan(v.Value, v.Value.Length > 2 ? 16 : 8); // Dumb
					break;

				case { Type: ShaderParameter.ParamType.SampledTexture2D, DefaultValue: ShaderValue.SampledTexture2D text }:
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

	public void AddInt(int value) => AddPrimitive(value, 4);

	public void AddFloat(float value) => AddPrimitive(value, 4);


	public void AddFloat2(float x, float y) => AddSpan(stackalloc float[] { x, y }, 8);
	public void AddFloat3(float x, float y, float z) => AddSpan(stackalloc float[] { x, y, z }, 12);
	public void AddFloat4(float x, float y, float z, float w) => AddSpan(stackalloc float[] { x, y, z, w }, 16);

	public void AddMatrix4x4(Matrix4x4 m)
	{
		AddFloat4(m.M11, m.M12, m.M13, m.M14);
		AddFloat4(m.M21, m.M22, m.M23, m.M24);
		AddFloat4(m.M31, m.M32, m.M33, m.M34);
		AddFloat4(m.M41, m.M42, m.M43, m.M44);
	}

	/// <summary> Writes an <see langword="unmanaged"/> value to the buffer with the given size and alignment. </summary>
	public void AddPrimitive<T>(T value, int baseAlign) where T : unmanaged
	{
		int size = Unsafe.SizeOf<T>(); // Much faster than Marshal.SizeOf

		EnsurePacking(size, baseAlign);
		EnsureCapacity(offset + size);
		MemoryMarshal.Write(data.AsSpan(offset, size), in value);
		offset += size;
	}

	/// <summary> Writes a <see cref="ReadOnlySpan{T}"/> of <see langword="unmanaged"/> values to the buffer with the given size and alignment. </summary>
	public void AddSpan<T>(ReadOnlySpan<T> values, int baseAlign) where T : unmanaged
	{
		int size = values.Length * Unsafe.SizeOf<T>(); // Much faster than Marshal.SizeOf

		EnsurePacking(size, baseAlign);
		EnsureCapacity(offset + size);
		MemoryMarshal.Cast<T, byte>(values).CopyTo(data.AsSpan(offset));
		offset += size;
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
		data.AsSpan(offset, aligned - offset).Clear();
		offset = aligned;
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
