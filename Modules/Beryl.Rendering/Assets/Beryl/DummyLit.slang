// DummyLit.slang
// An example shader that uses fake lighting data.

import Beryl.Core;
import Beryl.Attributes;

#include "Beryl/Preprocessor.slang"

SHADER_ATTRIBUTES(
	[ShaderPass("Opaque")]
)

SHADER_PARAMETERS(
    [DefaultFloat3(1, 1, 1)]
    float3 Color;

    [DefaultFloat(10)]
    float Intensity;

    [DefaultFloat3(0, 5, 3)]
    float3 Position;
)

[Shader("vertex")]
VertexOutput Vertex(VertexInput i)
{
    VertexOutput o;

    float4 worldPos = mul(Object.ModelMatrix, float4(i.Position, 1.0));

    o.Position = mul(
        Camera.ProjectionMatrix,
        mul(Camera.ViewMatrix, worldPos)
    );

    o.WorldPosition = worldPos.xyz;

    o.Normal = normalize(mul((float3x3)Object.ModelMatrix, i.Normal));

    return o;
}

[Shader("fragment")]
float4 Fragment(VertexOutput i) : SV_Target
{
    float3 normal = normalize(i.Normal);

    float3 toLight = Parameters.Position - i.WorldPosition;

    float distance = length(toLight);
    float3 lightDir = toLight / max(distance, 0.0001);

    float diffuse = max(dot(normal, lightDir), 0.0);
    float attenuation = 1.0 / (1.0 + distance * distance);
    float ambient = 0.1;
    float lighting = ambient + diffuse * Parameters.Intensity * attenuation;

    return float4(Parameters.Color * lighting, 1.0);
}
