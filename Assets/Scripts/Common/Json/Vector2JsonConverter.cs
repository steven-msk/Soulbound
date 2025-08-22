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
		JObject obj = JObject.Load(reader);
		return new Vector2(obj["x"]!.Value<float>(), obj["y"]!.Value<float>());
	}

	public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer) {
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(value.x);
		writer.WritePropertyName("y");
		writer.WriteValue(value.y);
		writer.WriteEndObject();
	}
}