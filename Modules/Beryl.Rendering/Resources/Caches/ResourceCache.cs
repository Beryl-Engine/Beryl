// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Standard;

namespace Beryl.Rendering.Resources.Caches;

/// <summary>
/// Base class for resource caches.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TResource">The type of the resource.</typeparam>
public abstract class ResourceCache<TSingletonOwner, TKey, TResource> where TSingletonOwner : new() where TKey : IAlwaysHashable, allows ref struct where TResource : IDisposable
{
	/// <summary> Singleton instance. </summary>
	public static TSingletonOwner Instance { get; } = new TSingletonOwner();

	public Dictionary<int, FrameCountedResource<TResource>> Cache { get; } = new();

	/// <summary> Creates a new <typeparamref name="TResource"/> for the given key. </summary>
	protected abstract TResource Create(TKey key);

	/// <summary> Gets or creates a <typeparamref name="TResource"/> for the given key. </summary>
	public FrameCountedResource<TResource> GetOrCreate(TKey key)
	{
		int hs = key.GetHashCode();
		if (Cache.TryGetValue(hs, out FrameCountedResource<TResource>? existing))
		{
			existing.Touch();
			return existing;
		}

		// We need to do lambda stuff in a seperate execution block to avoid allocations because lambda allocations will always happen if a method contains a lambda
		// (even if returning early)
		return InitializeNewCacheEntry(key);
	}

	/// <summary> Gets a <typeparamref name="TResource"/> for the given key. </summary>
	public FrameCountedResource<TResource>? Get(TKey key)
	{
		int hs = key.GetHashCode();
		if (Cache.TryGetValue(hs, out FrameCountedResource<TResource>? existing))
		{
			existing.Touch();
			return existing;
		}

		return null;
	}

	protected FrameCountedResource<TResource> InitializeNewCacheEntry(TKey key)
	{
		TResource created = Create(key);

		var value = new FrameCountedResource<TResource>(created);
		value.Touch();

		int hash = key.GetHashCode();
		value.Disposed += () => Cache.Remove(hash);
		Cache[hash] = value;

		return value;
	}
}
