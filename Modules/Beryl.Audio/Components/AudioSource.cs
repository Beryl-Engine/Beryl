// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Audio.Resources;
using Beryl.Common;
using Beryl.Scenes.Components;

namespace Beryl.Audio.Components;

public class AudioSource : Component
{
	private IAudioSource? source;

	/// <summary> The audio clip to play. </summary>
	public AudioClip? Clip { get; set; }

	/// <summary> Volume of the audio source. </summary>
	public float Volume { get; set; } = 1f;

	/// <summary> Pitch of the audio source. </summary>
	public float Pitch { get; set; } = 1f;

	/// <summary> Whether the audio source loops. </summary>
	public bool Loop { get; set; } = false;

	/// <summary> The line-of-sight obstruction level of the audio source. </summary>
	public float Obstruction { get; set; } = 0f;

	/// <summary> The acoustic occlusion level of the audio source. </summary>
	public float Occlusion { get; set; } = 0f;

	/// <summary> Whether the audio source's path to the listener is currently blocked. </summary>
	public bool Obstructed { get; set; } = false;

	/// <summary> The distance at which the audio source is heard at full volume. </summary>
	public float MinDistance { get; set; }

	/// <summary> The distance beyond which the audio source's volume no longer attenuates. </summary>
	public float MaxDistance { get; set; }

	/// <summary> How quickly the audio source's volume attenuates with distance. </summary>
	public float RolloffFactor { get; set; }

	/// <inheritdoc/>
	public override void Start()
	{
		var audio = ModuleManager.GetModule<AudioModule>();

		source = audio?.CreateSource();
		if (source != null)
		{
			source.Volume = Volume;
			source.Pitch = Pitch;
			source.Loop = Loop;
			source.MinDistance = MinDistance;
			source.MaxDistance = MaxDistance;
			source.RolloffFactor = RolloffFactor;
			if (Clip != null)
				source.SetClip(Clip);
		}
	}

	/// <summary> Plays the audio source. </summary>
	public void Play() => source?.Play();

	/// <summary> Stops the audio source. </summary>
	public void Stop() => source?.Stop();

	/// <inheritdoc/>
	public override void Update()
	{
		if (source == null)
			return;
		source.Obstruction = Obstructed ? 0.6f : 0f;
		source.Update(Entity.Transform.Position);
	}

	/// <inheritdoc/>
	public override void Destroy()
	{
		base.Destroy();
		var audio = ModuleManager.GetModule<AudioModule>();
		if (source != null)
			audio?.DestroySource(source);
	}
}
