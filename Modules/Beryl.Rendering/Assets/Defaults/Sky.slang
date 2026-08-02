// Sky.slang
// Procedural skybox shader.

import Beryl.Core;
import Beryl.Attributes;

#include "Beryl/Preprocessor.slang"

SHADER_ATTRIBUTES(
	[ShaderPass("Opaque")]
    [CullMode(CullingMode::Front)]
    [DepthWrite(DepthWriteMode::Off)]
    [DepthTest(DepthTestMode::LessOrEqual)]
)

SHADER_PARAMETERS(
    [DefaultFloat3(0.15, 0.35, 0.75)]
    float3 ZenithColor;

    [DefaultFloat3(0.7, 0.8, 0.9)]
    float3 HorizonColor;

    [DefaultFloat3(0.15, 0.15, 0.2)]
    float3 GroundColor;

    [DefaultFloat(2.0)]
    float SkyExponent;

    [DefaultFloat3(0.4, 0.7, 0.5)]
    float3 SunDirection;

    [DefaultFloat3(1.0, 0.95, 0.85)]
    float3 SunColor;

    [DefaultFloat(512.0)]
    float SunSize;

    [DefaultFloat(32.0)]
    float SunGlowFalloff;
)

struct SkyVertexOutput
{
    float4 Position : SV_Position;
    float3 Direction;
};

[Shader("vertex")]
SkyVertexOutput Vertex(VertexInput i)
{
    SkyVertexOutput o;

    float3x3 viewRotation = (float3x3)Camera.ViewMatrix;
    float4 viewPos = float4(mul(viewRotation, i.Position), 1.0);

    o.Position = mul(Camera.ProjectionMatrix, viewPos);
    o.Position.z = o.Position.w;

    o.Direction = i.Position;

    return o;
}

[Shader("fragment")]
float4 Fragment(SkyVertexOutput i) : SV_Target
{
    float3 dir = normalize(i.Direction);

    float heightFactor = dir.y;
    float3 skyColor;

    if (heightFactor > 0.0)
    {
        float t = pow(saturate(heightFactor), 1.0 / Parameters.SkyExponent);
        skyColor = lerp(Parameters.HorizonColor, Parameters.ZenithColor, t);
    }
    else
    {
        float t = pow(saturate(-heightFactor), 1.0 / Parameters.SkyExponent);
        skyColor = lerp(Parameters.HorizonColor, Parameters.GroundColor, t);
    }

    float3 sunDir = normalize(Parameters.SunDirection);
    float sunDot = saturate(dot(dir, sunDir));

    float sunDisc = smoothstep(1.0 - (1.0 / Parameters.SunSize), 1.0, sunDot);

    float sunGlow = pow(sunDot, Parameters.SunGlowFalloff);

    float3 color = skyColor;
    color += Parameters.SunColor * sunGlow * 0.5;
    color = lerp(color, Parameters.SunColor, sunDisc);

    return float4(color, 1.0);
}
