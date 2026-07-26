// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using System.Buffers;

namespace Beryl.Common.Standard;

/// <summary>
/// An array that is automatically rented from an <see cref="ArrayPool{T}"/> when initializing and returned when <see cref="Dispose"/> is called.
/// </summary>
/// <remarks>
/// Whilst <see cref="RentedArray{T}"/> garantees that allocations are kept to a minimum, <see langword="stackalloc"/> is cheaper and should be used in place of <see cref="RentedArray{T}"/> when <typeparamref name="T"/> is stack-compatible.
/// </remarks>
/// <typeparam name="T">The type of the objects in the array.</typeparam>
public readonly ref struct RentedArray<T> : IDisposable
{
	/// <summary> The rented <typeparamref name="T"/> array. </summary>
	public T[] Array { get; }

	/// <summary> Initializes a new instance of the <see cref="RentedArray{T}"/> struct with an empty array. </summary>
	public RentedArray() => Array = ArrayPool<T>.Shared.Rent(0);

	/// <summary> Initializes a new instance of the <see cref="RentedArray{T}"/> struct with the specified count. </summary>
	/// <param name="count">The number of elements in the array.</param>
	public RentedArray(int count) => Array = ArrayPool<T>.Shared.Rent(count);

	/// <inheritdoc/>
	public void Dispose() => ArrayPool<T>.Shared.Return(Array);
}
