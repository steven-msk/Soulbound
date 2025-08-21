using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

public sealed class BoundsInt2DJsonConverter : JsonConverter<BoundsInt2D> {
	public override BoundsInt2D ReadJson(JsonReader reader, Type objectType, BoundsInt2D existingValue, bool hasExistingValue, JsonSerializer serializer) {
		if (reader.TokenType == JsonToken.Null) {
			return default;
		} else if (reader.TokenType == JsonToken.StartArray) {
			JArray array = JArray.Load(reader);
			return new BoundsInt2D() {
				xMin = (int)array[0],
				yMin = (int)array[1],
				xMax = (int)array[2],
				yMax = (int)array[3]
			};
		} else if (reader.TokenType == JsonToken.StartObject) {
			JObject obj = JObject.Load(reader);
			return new BoundsInt2D() {
				xMin = obj["xMin"]!.Value<int>(),
				yMin = obj["yMin"]!.Value<int>(),
				xMax = obj["xMax"]!.Value<int>(),
				yMax = obj["yMax"]!.Value<int>()
			};
		}
		throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing BoundsInt2D.");
	}

	public override void WriteJson(JsonWriter writer, BoundsInt2D value, JsonSerializer serializer) {
		writer.WriteStartArray();
		serializer.Serialize(writer, value.xMin);
		serializer.Serialize(writer, value.yMin);
		serializer.Serialize(writer, value.xMax);
		serializer.Serialize(writer, value.yMax);
		writer.WriteEndArray();
	}
}
