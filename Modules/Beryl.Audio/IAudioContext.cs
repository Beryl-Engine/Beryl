// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Audio;

/// <summary>
/// Represents an audio context responsible for managing audio sources.
/// </summary>
public interface IAudioContext
{
	/// <summary> Creates a new audio source and adds it to the context. </summary>
	/// <returns> The created audio source. </returns>
	IAudioSource CreateSource();

	/// <summary> Destroys an audio source and removes it from the context. </summary>
	/// <param name="source"> The audio source to remove. </param>
	void DestroySource(IAudioSource source);

	/// <summary> Creates a new audio listener within the context. </summary>
	/// <returns> The created audio listener. </returns>
	IAudioListener CreateListener();

	/// <summary> Destroys an audio listener and removes it from the context. </summary>
	/// <param name="listener"> The audio listener to remove. </param>
	void DestroyListener(IAudioListener listener);

	/// <summary> Sets the active audio listener for the context. </summary>
	/// <param name="listener"> The audio listener to make current. </param>
	void SetListener(IAudioListener? listener);

	/// <summary> Sets the distance attenuation model used for spatial audio calculations. </summary>
	/// <param name="model"> The distance model to apply. </param>
	void SetDistanceModel(AudioDistanceModel model);
}
