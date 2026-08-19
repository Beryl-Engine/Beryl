// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

using Beryl.Common.Standard;
using Beryl.Common.Utility;
using Beryl.Math;
using Beryl.RHI;
using Beryl.RHI.Resources;
using Beryl.Rendering.Resources;
using Beryl.Rendering.Resources.Caches;

namespace Beryl.Rendering;

/// <summary>
/// Extension methods for <see cref="ICommandBuffer"/> providing high-level drawing operations.
/// </summary>
public static class CommandBufferExtensions
{
	extension(ICommandBuffer cmd)
	{
		/// <summary> Sets a constant buffer of name <paramref name="bufferName"/> to <paramref name="data"/>. </summary>
		public void SetConstantBuffer(string bufferName, ReadOnlySpan<byte> data)
		{
			NamedBufferDescriptor key = new(bufferName, (uint)data.Length, BufferUsage.UniformBuffer | BufferUsage.Dynamic);
			FrameCountedResource<IBuffer> buffer = NamedBufferCache.Instance.GetOrCreate(key);

			if (buffer.Resource.SizeInBytes != data.Length)
			{
				BerylConsole.Warning($"Constant buffer '{bufferName}' is {buffer.Resource.SizeInBytes} bytes, but {data.Length} bytes were written.");
				return;
			}

			cmd.UpdateBuffer(buffer.Resource, 0, data);
		}

		/// <summary> Binds a shader's resources. </summary>
		public void BindShaderResources(Shader shader, bool compute = false)
		{
			foreach (var group in shader.ResourceGroups)
			{
				using RentedArray<IBindableResource> boundResources = new(group.Resources.Length);

				int index = 0;

				foreach (var resource in group.Resources)
				{
					var buffer = NamedBufferCache.Instance.Get(new NamedBufferDescriptor(resource.Name, resource.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
					if (buffer?.Resource == null)
					{
						BerylConsole.Warning($"Could not find constant buffer '{resource.Name}'.");
						buffer = NamedBufferCache.Instance.GetOrCreate(new NamedBufferDescriptor(resource.Name, resource.SizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
					}

					boundResources.Array[index++] = buffer.Resource;
				}

				var layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutDescriptor(group.LayoutElements.AsSpan()));

				var setKey = new ResourceDescriptor(layout.Resource, boundResources.Array.AsSpan(0, index));
				var set = ResourceSetCache.Instance.GetOrCreate(setKey);

				if (compute)
					cmd.SetComputeResourceSet(group.Set, set.Resource);
				else
					cmd.SetGraphicsResourceSet(group.Set, set.Resource);
			}
		}


		/// <summary> Draws a mesh. </summary>
		public void DrawImmediate(ReadOnlySpan<Vector3> vertices, ReadOnlySpan<Vector3> normals, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<uint> indices, Matrix4x4 transform, Material material)
		{
			if (cmd.CurrentFramebuffer == null)
			{
				BerylConsole.Warning("Command Buffer requested draw without a target framebuffer.");
				return;
			}

			cmd.SetConstantBuffer("Parameters", Shader.DefaultsProvider.GetMaterialBuffer(material));
			cmd.SetConstantBuffer("Object", Shader.DefaultsProvider.GetObjectBuffer(transform));

			var pipelineKey = new PipelineDescriptor(
				passName: material.Shader.Pass,
				vertShader: new ShaderDescriptor(material.Shader.StageBytecode[ShaderStages.Vertex], ShaderStages.Vertex),
				fragShader: new ShaderDescriptor(material.Shader.StageBytecode[ShaderStages.Fragment], ShaderStages.Fragment),
				output: cmd.CurrentFramebuffer,
				resourceGroups: material.Shader.ResourceGroups.AsSpan(),
				cullingMode: material.Shader.CullingMode,
				depthInfo: new DepthInfo(material.Shader.DepthWrite, material.Shader.DepthComparison)
			);

			FrameCountedResource<RenderItem> renderItem = RenderItemCache.Instance.GetOrCreate(new RenderItemKey(vertices, normals, uvs, indices, pipelineKey));

			cmd.SetPipeline(renderItem.Resource.Pipeline.Resource);
			cmd.SetVertexBuffer(0, renderItem.Resource.VertexBuffer);
			cmd.SetIndexBuffer(renderItem.Resource.IndexBuffer);

			BindShaderResources(cmd, material.Shader);

			cmd.DrawIndexed(renderItem.Resource.IndexCount);
		}

		/// <summary> Draws a <see cref="IClientRenderable"/> with the given transform <see cref="Matrix4x4"/> and <see cref="Material"/>. </summary>
		public void DrawImmediate(IClientRenderable renderable, Matrix4x4 transform) => cmd.DrawImmediate(renderable.Vertices, renderable.Normals, renderable.UVs, renderable.Indices, transform, renderable.Material);
	}
}
