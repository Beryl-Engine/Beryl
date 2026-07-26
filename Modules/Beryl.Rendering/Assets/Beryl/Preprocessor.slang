// Preprocessor.slang
// Provides macros and definitions that can be included by Beryl shaders.

module Preprocessor;

// -----------------------------------------------------------------------------
// Macros.
// -----------------------------------------------------------------------------

// Defines a parameter block that can be read from the engine.
#define SHADER_PARAMETERS(params)                                                                                      \
    struct ShaderParameters                                                                                            \
    {                                                                                                                  \
        params                                                                                                         \
    };                                                                                                                 \
    ParameterBlock<ShaderParameters> Parameters;

// Allows applying attributes at a shader level.
#define SHADER_ATTRIBUTES(attributes)                                                                                      \
	attributes                                                                                           \
    struct ShaderAttributes {};