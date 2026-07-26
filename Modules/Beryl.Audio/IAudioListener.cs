// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Math;

namespace Beryl.Audio;

/// <summary>
/// Represents an audio listener within the audio context.
/// </summary>
public interface IAudioListener
{
	/// <summary> The world-space position of the listener. </summary>
	Vector3 Position { get; set; }

	/// <summary> The forward orientation vector of the listener. </summary>
	Vector3 Forward { get; set; }

	/// <summary> The up orientation vector of the listener. </summary>
	Vector3 Up { get; set; }

	/// <summary> The master gain/volume multiplier of the listener. </summary>
	float Gain { get; set; }

	void Update();
}
