// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.Common.Utility;

namespace Beryl.Rendering.Resources;

public interface IFrameCountedResource
{
	/// <summary> Called when the resource is disposed. </summary>
	event Action? Disposed;

	/// <summary> Tries to dispose the resource. </summary>
	/// <param name="currentFrame">The current frame. </param>
	/// <returns> True if the resource was disposed. </returns>
	bool TryDispose(ulong currentFrame);

	/// <summary> Force releases the GPU resource. </summary>
	void ReleaseGPUResource();

	/// <summary> Force marks the resource as being used this frame. </summary>
	void Touch();
}

/// <summary>
/// GPU Resource that is automatically disposed after it goes unused for a specified number of frames.
/// </summary>
/// <typeparam name="T">The underlying resource type.</typeparam>
public class FrameCountedResource<T> : IFrameCountedResource where T : IDisposable
{
	/// <summary> The number of frames to wait before disposing the resource. Defaults to 200. </summary>
	public uint FrameDisposalBuffer { get; init; } = 200;

	/// <summary> The last frame the resource was used or <see langword="null"/> if never used. </summary>
	internal ulong? LastUsedFrame { get; private set; } = null;

	/// <summary> The underlying resource. </summary>
	public T Resource
	{
		get { Touch(); return field; }
		private set => field = value;
	}

	/// <inheritdoc/>
	public event Action? Disposed;

	/// <summary> Initializes a new instance of the <see cref="FrameCountedResource{T}"/> class. </summary>
	public FrameCountedResource(T resource)
	{
		Resource = resource;
		FrameDisposalQueue.ActiveResources.Add(this);
	}

	/// <inheritdoc/>
	public bool TryDispose(ulong currentFrame)
	{
		if (LastUsedFrame is null)
			return false;

		if (currentFrame - LastUsedFrame.Value > FrameDisposalBuffer)
		{
			BerylConsole.Log($"Disposing {typeof(T).Name} ({Resource})");
			Disposed?.Invoke();
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
	public void ReleaseGPUResource() => Resource.Dispose();

	/// <inheritdoc/>
	public void Touch() => LastUsedFrame = ModuleManager.GetModule<RenderingModule>()?.CurrentFrame;
}
