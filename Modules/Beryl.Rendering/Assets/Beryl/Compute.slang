// Compute.slang
// Provides GPU-side access into the Scene.

import Beryl.Core;
import Beryl.Attributes;

#include "Beryl/Preprocessor.slang"

SHADER_ATTRIBUTES(
	[ShaderPass("Opaque")]
)

SHADER_PARAMETERS(
	[DefaultFloat3(0.15, 0.35, 0.75)]
    float3 ZenithColor;
)


[Shader("compute")]
[NumThreads(4, 1, 1)]
void ComputeStuff(uint3 thread: SV_DispatchThreadID)
{
    printf("Ran Compute shader on GPU thread %v3u\n", thread);
}
