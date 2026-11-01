// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Resources;
using Beryl.Common.Resources.DefaultProvider;
using Beryl.RHI.Resources;
using System.Collections.Immutable;

using StbImageSharp;

namespace Beryl.Rendering.Resources;

/// <summary>
/// Represents an image used in a material.
/// </summary>
public class Texture : Resource
{
	/// <summary> The pixel format used by this <see cref="Texture"/>. </summary>
	public TextureFormat Format { get; }

	public int Width { get; }
	public int Height { get; }

	/// <summary> The raw pixel data. </summary>
	public ImmutableArray<byte> Data
	{
		get;
		set
		{
			if (value.IsDefault || value.Length != Width * Height * GetPixelSize(Format))
				throw new ArgumentException("Data length does not match texture size.", nameof(value));

			field = value;
			ID = Guid.NewGuid();
		}
	} = [];

	/// <summary> Sampler settings used when sampling this <see cref="Texture"/>. </summary>
	public Sampler Sampler { get; }

	#region Defaults
	/// <summary> A one-pixel opaque white texture. </summary>
	public static Texture White => new(1, 1, TextureFormat.RGBA8, [255, 255, 255, 255], new Sampler());
	#endregion

	/// <summary> Creates a new <see cref="Texture"/> with optional pixel data. </summary>
	/// <param name="width">Texture width in pixels.</param>
	/// <param name="height">Texture height in pixels.</param>
	/// <param name="format">Pixel format.</param>
	/// <param name="initialData">Optional initial pixel data.</param>
	public Texture(int width, int height, TextureFormat format, byte[]? initialData = null, Sampler sampler = default)
	{
		Format = format;
		Width = width;
		Height = height;
		Data = (initialData ?? new byte[width * height * GetPixelSize(format)]).ToImmutableArray();

		Sampler = sampler;
	}

	/// <summary> Clears the texture to a specific color. </summary>
	public void Clear(byte r, byte g, byte b, byte a = 255)
	{
		int pixelSize = GetPixelSize(Format);
		var data = ImmutableArray.CreateBuilder<byte>(Data.Length);

		for (int i = 0; i < Data.Length; i += pixelSize)
		{
			data.Add(r);
			data.Add(g);
			data.Add(b);
			if (pixelSize == 4)
				data.Add(a);
		}
		Data = data.MoveToImmutable();
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
