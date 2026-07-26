// Debug.slang
// Debug shader that uses no cbuffers.

import Beryl.Core;
import Beryl.Attributes;

#include "Beryl/Preprocessor.slang"

SHADER_ATTRIBUTES(
    [ShaderPass("Opaque")]
)

SHADER_PARAMETERS()

[Shader("vertex")]
VertexOutput Vertex(VertexInput i)
{
    VertexOutput o;

    o.Position = float4(i.Position, 1.0);

    o.Normal = i.Normal;
    o.UV = i.UV;

    return o;
}

[Shader("fragment")]
float4 Fragment(VertexOutput i) : SV_Target
{
    return float4(1, 1, 1, 1);
}
