// Rect.slang
// Screen-space GUI rectangle shader.

import Beryl.Core;
import Beryl.Attributes;

#include "Beryl/Preprocessor.slang"

SHADER_ATTRIBUTES(
    [ShaderPass("GUI")]
    [CullMode(CullingMode.None)]
    [DepthWrite(DepthWriteMode.Off)]
    [DepthTest(DepthTestMode.Always)]
)

SHADER_PARAMETERS(
    [DefaultFloat4(1, 1, 1, 1)]
    float4 Color;

	[DefaultFloat2(1, 1)]
    float2 RectPosition;

	[DefaultFloat2(1, 1)]
    float2 RectSize;

	[DefaultFloat2(1, 1)]
    float2 ViewportSize;
)

[Shader("vertex")]
VertexOutput Vertex(VertexInput i)
{
    float2 corner = i.Position.xy;
    float2 pixelPosition = Parameters.RectPosition + corner * Parameters.RectSize;
    float2 clipPosition = float2(pixelPosition.x / Parameters.ViewportSize.x * 2.0 - 1.0, 1.0 - pixelPosition.y / Parameters.ViewportSize.y * 2.0);

    VertexOutput o;
    o.Position = float4(clipPosition, 0.0, 1.0);
    o.UV = corner;
    o.Normal = float3(0.0);
    o.WorldPosition = float3(0.0);
    return o;
}

[Shader("fragment")]
float4 Fragment(VertexOutput i) : SV_Target
{
    return Parameters.Color;
}
