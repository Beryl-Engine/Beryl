// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.Common.Utility;
using Beryl.Rendering;
using Beryl.VirtualReality.OpenXR;

namespace Beryl.VirtualReality;

[InitializeAfter(typeof(RenderingModule))]
public class VirtualRealityModule : BaseModule
{
	/// <summary> The currently connected <see cref="VirtualRealityDevice"/>. </summary>
	public VirtualRealityDevice Device { get; set; } = new OpenXRDevice();

	/// <inheritdoc/>
	public override void Initialize()
	{
		Application.OnUpdate += Update;

		try
		{
			Device.Initialize();

			BerylConsole.Log($"Connected Virtual Reality device: {Device.Name}");
		}
		catch (Exception ex)
		{
			BerylConsole.Exception(ex);
			BerylConsole.Warning("Exception occured during Virtual Reality device initialization, switching to emulation.");
		}
	}

	/// <inheritdoc/>
	public override void Dispose()
	{
		base.Dispose();

		Application.OnUpdate -= Update;
	}

	public void Update() => Device.Update();
}
