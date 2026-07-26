// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Resources.DefaultProvider;
using Beryl.RHI.Resources;

using StbImageSharp;

namespace Beryl.Rendering.Resources;

/// <summary>
/// Represents an image used in a material.
/// </summary>
public class Texture
{
	public TextureFormat Format { get; }

	public int Width { get; }
	public int Height { get; }

	public byte[] Data { get; private set; }

	/// <summary> Sampler settings used when sampling this <see cref="Texture"/>. </summary>
	public Sampler Sampler { get; }

	/// <summary> Creates a new <see cref="Texture"/> with optional pixel data. </summary>
	/// <param name="width">Texture width in pixels.</param>
	/// <param name="height">Texture height in pixels.</param>
	/// <param name="format">Pixel format.</param>
	/// <param name="initialData">Optional initial pixel data.</param>
	public Texture(int width, int height, TextureFormat format, byte[]? initialData = null, Sampler sampler = default)
	{
		Format = format;
		Data = initialData ?? new byte[width * height * GetPixelSize(format)];
		Width = width;
		Height = height;

		Sampler = sampler;
	}

	/// <summary> Sets the raw pixel data. </summary>
	/// <param name="data">Byte array of pixel data matching texture format.</param>
	public void SetData(byte[] data)
	{
		if (data.Length != Data.Length)
			throw new ArgumentException("Data length does not match texture size.");

		Data = data;
	}

	/// <summary> Gets a copy of the raw pixel data. </summary>
	public byte[] GetData()
	{
		byte[] copy = new byte[Data.Length];
		Array.Copy(Data, copy, Data.Length);
		return copy;
	}

	/// <summary> Clears the texture to a specific color. </summary>
	public void Clear(byte r, byte g, byte b, byte a = 255)
	{
		int pixelSize = GetPixelSize(TextureFormat.RGBA8);

		for (int i = 0; i < Data.Length; i += pixelSize)
		{
			Data[i + 0] = r;
			Data[i + 1] = g;
			Data[i + 2] = b;
			Data[i + 3] = a;
		}
	}

	private static int GetPixelSize(TextureFormat format)
	{
		switch (format)
		{
			case TextureFormat.RGBA8:
				return 4;
			case TextureFormat.RGB8:
				return 3;
			default:
				throw new ArgumentException("Invalid texture format.");
		}
	}
}


[ResourceLoader]
internal sealed class TextureLoader : ResourceLoader<Texture>
{
	/// <inheritdoc/>
	public override Texture? LoadResource(byte[] data, string? name = null)
	{
		var image = ImageResult.FromMemory(data, ColorComponents.RedGreenBlueAlpha);

		return new Texture(image.Width, image.Height, TextureFormat.RGBA8, image.Data, new Sampler());
	}
}
