using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

public sealed class Vector2JsonConverter : JsonConverter<Vector2> {
	public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer) {
		if (reader.TokenType == JsonToken.StartArray) {
			JArray array = JArray.Load(reader);
			return new Vector2(array[0]!.Value<float>(), array[1]!.Value<float>());
		} else if (reader.TokenType == JsonToken.StartObject) {
			JObject obj = JObject.Load(reader);
			return new Vector2(obj["x"]!.Value<float>(), obj["y"]!.Value<float>());
		}
		throw new JsonSerializationException($"Invalid Vector2 json format");
	}

	public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer) {
		writer.WriteStartArray();
		writer.WriteValue(value.x);
		writer.WriteValue(value.y);
		writer.WriteEndArray();
	}
}