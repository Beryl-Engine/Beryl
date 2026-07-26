// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.RHI.Resources;

/// <summary>
/// A GPU buffer.
/// </summary>
public interface IBuffer : IBindableResource
{
	/// <summary> The buffer size in bytes. </summary>
	uint SizeInBytes { get; }
}

/// <summary>
/// A GPU framebuffer.
/// </summary>
public interface IFramebuffer : IGPUResource { }

/// <summary>
/// A GPU shader.
/// </summary>
public interface IShader : IGPUResource { }

/// <summary>
/// A GPU texture.
/// </summary>
public interface ITexture : IBindableResource { }

/// <summary>
/// A view into a GPU texture.
/// </summary>
public interface ITextureView : IBindableResource { }

/// <summary>
/// A GPU sampler.
/// </summary>
public interface ISampler : IBindableResource { }

/// <summary>
/// A shader resource layout.
/// </summary>
public interface IResourceLayout : IGPUResource { }

/// <summary>
/// Bound resources matching an <see cref="IResourceLayout"/>.
/// </summary>
public interface IResourceSet : IGPUResource { }

/// <summary>
/// A graphics pipeline.
/// </summary>
public interface IPipeline : IGPUResource { }

/// <summary>
/// A GPU resource that can be bound to a shader.
/// </summary>
public interface IBindableResource : IGPUResource { }

/// <summary>
/// Supported GPU buffer usages.
/// </summary>
[Flags]
public enum BufferUsage : byte
{
	VertexBuffer = 1,
	IndexBuffer = 2,
	UniformBuffer = 4,
	Dynamic = 8
}

/// <summary>
/// The kind of shader resource.
/// </summary>
public enum ResourceKind : byte
{
	UniformBuffer
}

/// <summary>
/// All stages of a shader.
/// </summary>
[Flags]
public enum ShaderStages : byte
{
	Vertex = 1,
	Fragment = 2
}
