// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Silk.NET.Vulkan;

namespace Beryl.RHI.VulkanBackend;

/// <summary>
/// Extensions for Silk.NET.Vulkan.
/// </summary>
internal static class VulkanExtensions
{
	extension(Result res)
	{
		/// <summary> Throws an exception if the result is not <see cref="Result.Success"/>. </summary>
		public Result ThrowIfFailed()
		{
			if (res != Result.Success)
				throw new Exception("Vulkan call failed.");

			return res;
		}
	}
}
