using System;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace SoulboundBackend.Common.Json {
	public sealed class ColorJsonConverter : JsonConverter<UnityEngine.Color> {
		public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer) {
			string hex = reader.ReadAsString();
			if (ColorUtility.TryParseHtmlString(hex, out var color)) {
				return color;
			}
			throw new JsonException($"Invalid color hex: {hex}");
		}

		public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer) {
			Color32 c = value;
			string hex = (c.a == 255) ? $"#{c.r:X2}{c.g:X2}{c.b:X2}" : $"#{c.r:X2}{c.g:X2}{c.a:X2}";
			writer.WriteValue(hex);
		}
	}
}
