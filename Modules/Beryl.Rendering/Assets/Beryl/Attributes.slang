// Attributes.slang
// Provides definitions for custom engine shader attributes.

module Attributes;

// -----------------------------------------------------------------------------
// Shader Parameters.
// -----------------------------------------------------------------------------

#define SHADER_PARAMETER [__AttributeUsage(_AttributeTargets.Struct)]

/// @brief Specifies which rendering pass this shader will be used in.
SHADER_PARAMETER
public struct ShaderPassAttribute
{
    string passName;
};

/// @brief Specifies how objects using this shader will be culled.
///
/// Valid values are:
/// - `"None"`: Disable face culling.
/// - `"Front"`: Cull front-facing polygons.
/// - `"Back"`: Cull back-facing polygons.
SHADER_PARAMETER
public struct CullModeAttribute
{
    string modeName;
};

// -----------------------------------------------------------------------------
// Defaults.
// -----------------------------------------------------------------------------

#define DEFAULT_USAGE [__AttributeUsage(_AttributeTargets.Var)]

DEFAULT_USAGE
public struct DefaultFloatAttribute
{
	float value;
};

DEFAULT_USAGE
public struct DefaultIntAttribute
{
	int value;
};

DEFAULT_USAGE
public struct DefaultFloat2Attribute
{
    float x, y;
};

DEFAULT_USAGE
public struct DefaultFloat3Attribute
{
    float x, y, z;
};

DEFAULT_USAGE
public struct DefaultFloat4Attribute
{
    float x, y, z, w;
};

DEFAULT_USAGE
public struct DefaultTextureAttribute
{
    string path;
};
