using System;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace SoulboundBackend.Common.Json {
	public sealed class Vector3JsonConverter : JsonConverter<Vector3> {
		public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer) {
			JObject obj = JObject.Load(reader);
			return new Vector3(obj["x"]!.Value<float>(), obj["y"]!.Value<float>(), obj["z"]!.Value<float>());
		}

		public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer) {
			writer.WriteStartObject();
			writer.WritePropertyName("x");
			writer.WriteValue(value.x);
			writer.WritePropertyName("y");
			writer.WriteValue(value.y);
			writer.WritePropertyName("z");
			writer.WriteValue(value.z);
			writer.WriteEndObject();
		}
	}
}
