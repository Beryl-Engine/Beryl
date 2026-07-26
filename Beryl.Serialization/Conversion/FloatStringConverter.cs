// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Serialization.Conversion;

[StringConverter(typeof(float))]
internal sealed class FloatStringConverter : StringConverter<float>
{
	/// <inheritdoc/>
	public override float ConvertFromString(string value) => float.Parse(value);

	/// <inheritdoc/>
	public override string ConvertToString(float value) => value.ToString();
}
