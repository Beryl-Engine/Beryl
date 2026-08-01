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

    [DefaultFloat(0.6)]
    float Intensity;

    [DefaultFloat3(0.4, 0.7, 0.5)]
    float3 SunDirection;

    [DefaultFloat3(1, 1, 1)]
    float3 SpecularColor;

    [DefaultFloat(0.2)]
    float SpecularIntensity;

    [DefaultFloat(16)]
    float SpecularPower;
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

    float3 lightDir = normalize(Parameters.SunDirection);

    float diffuse = max(dot(normal, lightDir), 0.0);
    float ambient = 0.4;

    float3 viewDir = normalize(Camera.WorldPosition - i.WorldPosition);
    float3 halfDir = normalize(lightDir + viewDir);
    float specAngle = max(dot(normal, halfDir), 0.0);
    float specular = pow(specAngle, Parameters.SpecularPower) * Parameters.SpecularIntensity;

    specular *= step(0.0001, diffuse);

    float lighting = ambient + diffuse * Parameters.Intensity;
    float3 color = Parameters.Color * lighting + Parameters.SpecularColor * specular;

    return float4(color, 1.0);
}
