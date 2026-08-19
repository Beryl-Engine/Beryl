// Compute.slang
// Debugging Compute stuff

import Beryl.Core;
import Beryl.Attributes;

#include "Beryl/Preprocessor.slang"

SHADER_ATTRIBUTES()
SHADER_PARAMETERS(
    int Test;
)

[Shader("compute")]
[NumThreads(4, 1, 1)]
void ComputeStuff(uint3 thread: SV_DispatchThreadID)
{
    printf("Ran Compute shader on GPU thread %v3u\n", thread);
    printf("Test: %d\n", Parameters.Test);
}
