// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Audio.Resources;
using Beryl.Math;

namespace Beryl.Audio;

/// <summary>
/// Represents an audio source within the audio context.
/// </summary>
public interface IAudioSource
{
	/// <summary> Volume of the audio source. </summary>
	float Volume { get; set; }

	/// <summary> Pitch of the audio source. </summary>
	float Pitch { get; set; }

	/// <summary> Whether the audio source loops. </summary>
	bool Loop { get; set; }

	/// <summary> Whether the audio source is currently playing. </summary>
	bool IsPlaying { get; }

	/// <summary> The line-of-sight obstruction level of the audio source. </summary>
	float Obstruction { get; set; }

	/// <summary> The acoustic occlusion level of the audio source. </summary>
	float Occlusion { get; set; }

	/// <summary> Whether the audio source's path to the listener is currently blocked. </summary>
	bool Obstructed { get; set; }

	/// <summary> The distance at which the audio source is heard at full volume. </summary>
	public float MinDistance { get; set; }

	/// <summary> The distance beyond which the audio source's volume no longer attenuates. </summary>
	public float MaxDistance { get; set; }

	/// <summary> How quickly the audio source's volume attenuates with distance. </summary>
	public float RolloffFactor { get; set; }

	/// <summary> Sets the audio clip to be used for playback. </summary>
	/// <param name="clip"> The audio clip to assign. </param>
	void SetClip(AudioClip clip);

	/// <summary> Plays the audio source. </summary>
	void Play();

	/// <summary> Stops the audio source. </summary>
	void Stop();

	/// <summary> Destroys the audio source. </summary>
	void Destroy();

	void Update(Vector3 position);
}
