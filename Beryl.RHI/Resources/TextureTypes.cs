// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.RHI.Resources;

/// <summary>
/// Texture pixel format.
/// </summary>
public enum TextureFormat
{
	RGBA8,
	RGB8
}

/// <summary>
/// Texture sampling filter.
/// </summary>
public enum SamplerFilter
{
	Nearest,
	Linear
}

/// <summary>
/// Texture wrap mode.
/// </summary>
public enum TextureWrapMode
{
	Repeat,
	Clamp
}

/// <summary>
/// Describes sampler state for texture sampling.
/// </summary>
public struct Sampler
{
	public SamplerFilter Filter { get; set; } = SamplerFilter.Linear;
	public TextureWrapMode WrapU { get; set; } = TextureWrapMode.Repeat;
	public TextureWrapMode WrapV { get; set; } = TextureWrapMode.Repeat;

	public Sampler() { }
}
