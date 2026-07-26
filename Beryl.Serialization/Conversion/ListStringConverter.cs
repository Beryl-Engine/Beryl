// This file is part of the Beryl Game Engine.
// Licensed under the MIT license. (https://github.com/Beryl-Engine/Beryl/blob/main/LICENSE)

namespace Beryl.Serialization.Conversion;

[StringConverter(typeof(List<>))]
internal sealed class ListStringConverter<T> : StringConverter<List<T>>
{
	private const char Separator = '|';

	/// <inheritdoc/>
	public override List<T> ConvertFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
			return new List<T>();

		object converter = StringConverterAttribute.GetConverter(typeof(T));
		var method = converter.GetType().GetMethod("ConvertFromString")!;

		return value.Split(Separator).Select(s => (T)method.Invoke(converter, new object[] { s })!).ToList();
	}

	/// <inheritdoc/>
	public override string ConvertToString(List<T> value)
	{
		if (value.Count == 0)
			return string.Empty;

		object converter = StringConverterAttribute.GetConverter(typeof(T));
		var method = converter.GetType().GetMethod("ConvertToString")!;

		return string.Join(Separator, value.Select(item => (string)method.Invoke(converter, new object[] { item! })!));
	}
}
