// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Silk.NET.OpenXR;

namespace Beryl.VirtualReality.OpenXR;

internal static class ResultExtensions
{
	extension(Result result)
	{
		/// <summary> Throws an exception if the result is not <see cref="Result.Success"/>. </summary>
		public void VerifySuccess()
		{
			if (result == Result.Success)
				return;

			throw new Exception($"OpenXR Error: {result}");
		}
	}
}
