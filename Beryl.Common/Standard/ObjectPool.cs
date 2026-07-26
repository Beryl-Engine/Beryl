// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Collections.Concurrent;
using Beryl.Common.Utility;

namespace Beryl.Common.Datatypes;

/// <summary>
/// Base class for managed objects that can be shared via renting and returning.
/// </summary>
/// <typeparam name="TObject">The object this pool manages.</typeparam>
public abstract class ObjectPool<TSingletonOwner, TObject> where TSingletonOwner : ObjectPool<TSingletonOwner, TObject>, new() where TObject : class
{
	/// <summary> The maximum size of the pool before overflowing. </summary>
	protected virtual int MaxPoolSize => 256;

	/// <summary> Shared <see cref="ObjectPool{TSingletonOwner, TObject}"/> instance. </summary>
	public static TSingletonOwner Shared { get; } = new TSingletonOwner();

	private readonly ConcurrentQueue<TObject> pool = new();

	/// <summary> Creates a new instance of <typeparamref name="TObject"/>. </summary>
	protected abstract TObject CreateObject();

	/// <summary> Resets the given <typeparamref name="TObject"/> to it's default state for reuse. </summary>
	protected virtual void ResetObject(TObject obj) { }

	/// <summary> Borrows a <typeparamref name="TObject"/> from the pool or creates a new one if pool has none free. </summary>
	/// <remarks> <typeparamref name="TObject"/>s obtained via this method should be returned via <see cref="Return"/> when no longer in use. </remarks>
	public TObject Rent()
	{
		if (!pool.TryDequeue(out TObject? obj))
			obj = CreateObject();

		ResetObject(obj);
		return obj;
	}

	/// <summary> Rents an object and wraps it in an allocation-free disposable structure. </summary>
	/// <remarks> <see cref="PooledObject{TObject, TPool}"/>s obtained via this method should be disposed when no longer in use. </remarks>
	public PooledObject<TObject, TSingletonOwner> RentAuto() => new(Rent());

	/// <summary> Returns the given <typeparamref name="TObject"/> to the pool. </summary>
	public void Return(TObject obj)
	{
		if (obj == null)
			return;

		if (pool.Count < MaxPoolSize)
			pool.Enqueue(obj);
		else
			BerylConsole.Warning($"Object Pool for {typeof(TObject).Name} is full! Discarding instance.");
	}
}

public readonly ref struct PooledObject<TObject, TPool> : IDisposable where TObject : class where TPool : ObjectPool<TPool, TObject>, new()
{
	/// <summary> The underlying rented object. </summary>
	public TObject Object { get; }

	public PooledObject(TObject obj) => Object = obj;

	/// <summary> Returns the held object back to the <typeparamref name="TPool"/>. </summary>
	public void Dispose()
	{
		if (Object != null)
			ObjectPool<TPool, TObject>.Shared.Return(Object);
	}

	public static implicit operator TObject(PooledObject<TObject, TPool> wrapper) => wrapper.Object;
}
