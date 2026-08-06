// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common;
using Beryl.Math;
using Beryl.Rendering.Components;
using Beryl.Rendering.Resources;
using Beryl.RHI;

namespace Beryl.Rendering;

/// <summary>
/// Interface for providing values for default <see cref="Resources.Shader"/> parameter blocks.
/// </summary>
public interface IShaderDefaultsProvider
{
	/// <summary> Gets a <see cref="ReadOnlySpan{T}"/> with data from <paramref name="camera"/> formatted for uploading via <see cref="ICommandBuffer.UpdateBuffer(RHI.Resources.IBuffer, uint, ReadOnlySpan{byte})"/>. </summary>
	ReadOnlySpan<byte> GetCameraBuffer(Camera camera);

	/// <summary> Gets a <see cref="ReadOnlySpan{T}"/> with data from <paramref name="transform"/> formatted for uploading via <see cref="ICommandBuffer.UpdateBuffer(RHI.Resources.IBuffer, uint, ReadOnlySpan{byte})"/>. </summary>
	ReadOnlySpan<byte> GetObjectBuffer(Matrix4x4 transform);

	/// <summary> Gets a <see cref="ReadOnlySpan{T}"/> with <paramref name="material"/> parameter data formatted for uploading via <see cref="ICommandBuffer.UpdateBuffer(RHI.Resources.IBuffer, uint, ReadOnlySpan{byte})"/>. </summary>
	ReadOnlySpan<byte> GetMaterialBuffer(Material material);
}

public class ShaderDefaultsProvider : IShaderDefaultsProvider
{
	private Matrix4x4 ClipCorrectionMatrix => (ModuleManager.GetModule<RenderingModule>()!.Device.Features & DeviceFeatures.ClipSpaceYInverted) != 0 ? Matrix4x4.CreateScale(1, -1, 1) : Matrix4x4.Identity;

	/// <inheritdoc/>
	public ReadOnlySpan<byte> GetCameraBuffer(Camera camera)
	{
		using var cameraBuffer = GPUBufferPool.Shared.RentAuto();
		cameraBuffer.Object.AddFloat3(camera.Entity.Transform.Position.X, camera.Entity.Transform.Position.Y, camera.Entity.Transform.Position.Z);
		cameraBuffer.Object.AddMatrix4x4(camera.ViewMatrix);
		cameraBuffer.Object.AddMatrix4x4(camera.ProjectionMatrix * ClipCorrectionMatrix);

		return cameraBuffer.Object.Data;
	}

	/// <inheritdoc/>
	public ReadOnlySpan<byte> GetObjectBuffer(Matrix4x4 transform)
	{
		using var objectBuffer = GPUBufferPool.Shared.RentAuto();
		objectBuffer.Object.AddMatrix4x4(transform);
		return objectBuffer.Object.Data;
	}

	/// <inheritdoc/>
	public ReadOnlySpan<byte> GetMaterialBuffer(Material material)
	{
		using var materialBuffer = GPUBufferPool.Shared.RentAuto();
		materialBuffer.Object.AddMaterialParameters(material);
		return materialBuffer.Object.Data;
	}
}
