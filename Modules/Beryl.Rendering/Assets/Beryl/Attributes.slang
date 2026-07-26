// Attributes.slang
// Provides definitions for custom engine shader attributes.

module Attributes;

// Defines which rendering pass this shader will be used in.
[__AttributeUsage(_AttributeTargets.Struct)]
public struct ShaderPassAttribute
{
    string passName;
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
