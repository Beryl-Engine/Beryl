// Attributes.slang
// Provides definitions for custom engine shader attributes.

module Attributes;

// -----------------------------------------------------------------------------
// Shader Attribute Enums.
// NOTE: Translation and usage of these are handled in Shader.cs,
// be careful when making changes!
// -----------------------------------------------------------------------------

public enum CullingMode
{
    None,
    Front,
    Back
};

public enum DepthWriteMode
{
    Off,
    On
};

public enum DepthTestMode
{
    Never,
    Always,
    Less,
    Equal,
    LessOrEqual,
    Greater,
    NotEqual,
    GreaterOrEqual
};

// -----------------------------------------------------------------------------
// Shader Attributes.
// -----------------------------------------------------------------------------

#define SHADER_ATTRIBUTE [__AttributeUsage(_AttributeTargets.Struct)]

/// @brief Specifies which rendering pass this shader will be used in.
SHADER_ATTRIBUTE
public struct ShaderPassAttribute
{
    string passName;
};

/// @brief Specifies how objects using this shader will be culled.
SHADER_ATTRIBUTE
public struct CullModeAttribute
{
    CullingMode mode;
};

/// @brief Specifies how objects using this shader will be written to the depth buffer.
SHADER_ATTRIBUTE
public struct DepthWriteAttribute
{
    DepthWriteMode mode;
};

/// @brief Specifies how objects using this shader will test against the depth buffer.
SHADER_ATTRIBUTE
public struct DepthTestAttribute
{
    DepthTestMode mode;
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
