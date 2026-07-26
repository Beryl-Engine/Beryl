// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Serialization.Conversion;

[StringConverter(typeof(bool))]
internal sealed class BoolStringConverter : StringConverter<bool>
{
	/// <inheritdoc/>
	public override bool ConvertFromString(string value) => value == "true";

	/// <inheritdoc/>
	public override string ConvertToString(bool value) => value ? "true" : "false";
}
