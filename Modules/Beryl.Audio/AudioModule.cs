// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Audio.Engines.OpenAL;
using Beryl.Common;

namespace Beryl.Audio;

/// <summary>
/// Module responsible for handling audio.
/// </summary>
public class AudioModule : BaseModule, IAudioContext
{
	private readonly IAudioContext context;

	/// <inheritdoc/>
	public IAudioSource CreateSource() => context.CreateSource();

	/// <inheritdoc/>
	public void DestroySource(IAudioSource source) => context.DestroySource(source);

	/// <inheritdoc/>
	public IAudioListener CreateListener() => context.CreateListener();

	/// <inheritdoc/>
	public void DestroyListener(IAudioListener listener) => context.DestroyListener(listener);

	/// <inheritdoc/>
	public void SetListener(IAudioListener? listener) => context.SetListener(listener);

	/// <inheritdoc/>
	public void SetDistanceModel(AudioDistanceModel model) => context.SetDistanceModel(model);

	/// <summary> Creates a new audio module with the given audio context. </summary>
	/// <param name="context"> The audio context to use. </param>
	public AudioModule(IAudioContext context) => this.context = context;

	/// <summary> Creates a new audio module with the default audio context. </summary>
	public AudioModule() => this.context = new OpenALContext();

	/// <inheritdoc/>
	public override void Initialize() => Application.OnUpdate += Update;

	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();

		Application.OnUpdate -= Update;
	}

	public void Update() { }
}

/// <summary> Defines how audio source volume attenuates with distance from the listener. </summary>
public enum AudioDistanceModel
{
	None,
	InverseDistance,
	InverseDistanceClamped,
	LinearDistance,
	LinearDistanceClamped,
	ExponentDistance,
	ExponentDistanceClamped
}
