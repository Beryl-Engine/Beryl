// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Serialization.Conversion;

[StringConverter(typeof(int))]
internal sealed class IntStringConverter : StringConverter<int>
{
	/// <inheritdoc/>
	public override int ConvertFromString(string value) => int.Parse(value);

	/// <inheritdoc/>
	public override string ConvertToString(int value) => value.ToString();
}
