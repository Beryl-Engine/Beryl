// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Input;

namespace Beryl.VirtualReality;

public static class InputModuleExtensions
{
	extension(InputModule input)
	{
		/// <summary> The primary (first connected) <see cref="VirtualRealityController"/> that belongs to the left hand. </summary>
		public VirtualRealityController? PrimaryHandLeft => input.Devices.OfType<VirtualRealityController>().FirstOrDefault(x => x.Handness == VirtualRealityController.HandType.Left);

		/// <summary> The primary (first connected) <see cref="VirtualRealityController"/> that belongs to the right hand. </summary>
		public VirtualRealityController? PrimaryHandRight => input.Devices.OfType<VirtualRealityController>().FirstOrDefault(x => x.Handness == VirtualRealityController.HandType.Right);
	}
}
