// Core.slang
// Provides abstractions and datatypes used for Beryl shaders.

module Core;

// -----------------------------------------------------------------------------
// Shared (Global) Parameters.
// -----------------------------------------------------------------------------

public struct CameraData
{
    public float3 WorldPosition;
    public float4x4 ViewMatrix;
    public float4x4 ProjectionMatrix;
};

/// @brief Information about the rendering Camera.
public ParameterBlock<CameraData> Camera;

public struct ObjectData
{
    public float4x4 ModelMatrix;
};

/// @brief Information about the object being rendered.
public ParameterBlock<ObjectData> Object;

// -----------------------------------------------------------------------------
// Shared Structs.
// -----------------------------------------------------------------------------

public struct VertexInput
{
    public float3 Position : POSITION;
    public float3 Normal : NORMAL;
    public float2 UV : TEXCOORD0;
};

public struct VertexOutput
{
    public float4 Position : SV_Position;
    public float3 Normal : NORMAL;
    public float2 UV : TEXCOORD0;

    public float3 WorldPosition;
};

/// @brief Wrapper for two-dimensional Textures and Samplers.
public struct SampledTexture2D<T> where T : ITexelElement
{
    public Texture2D<T> Texture;
    public SamplerState Sampler;

    public T Sample(float2 uv)
    {
        return Texture.Sample(Sampler, uv);
    }
};
