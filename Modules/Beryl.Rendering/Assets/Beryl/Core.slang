// Core.slang
// Provides abstractions and datatypes used for Beryl shaders.

module Core;

// -----------------------------------------------------------------------------
// Shared (Global) Parameters.
// -----------------------------------------------------------------------------

public struct CameraData
{
    public float4x4 ViewMatrix;
    public float4x4 ProjectionMatrix;
};

// Information about the rendering camera.
public ParameterBlock<CameraData> Camera;

public struct ObjectData
{
    public float4x4 ModelMatrix;
};

// Information about this object.
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

// Wrapper for Two-Dimensional Textures and Samplers.
public struct SampledTexture2D<T> where T : ITexelElement
{
    public Texture2D<T> Texture;
    public SamplerState Sampler;
};
