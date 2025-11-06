using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum PlayerModelType
{
	Ghost,
	Cube
	// 必要なら増やせる：Warrior, Mage など
}