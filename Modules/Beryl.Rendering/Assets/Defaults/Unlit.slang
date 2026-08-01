// Unlit.slang
// Default unlit shader.

import Beryl.Core;
import Beryl.Attributes;

#include "Beryl/Preprocessor.slang"

SHADER_ATTRIBUTES(
    [ShaderPass("Opaque")]
)

SHADER_PARAMETERS(
    [DefaultFloat4(1, 1, 1, 1)]
    float4 Color;
)

[Shader("vertex")]
VertexOutput Vertex(VertexInput i)
{
    VertexOutput o;

    float4 world = mul(Object.ModelMatrix, float4(i.Position, 1));
    float4 view = mul(Camera.ViewMatrix, world);

    o.Position = mul(Camera.ProjectionMatrix, view);

    o.Normal = 0;
    o.UV = 0;

    return o;
}

[Shader("fragment")]
float4 Fragment(VertexOutput i) : SV_Target
{
    return Parameters.Color;
}
