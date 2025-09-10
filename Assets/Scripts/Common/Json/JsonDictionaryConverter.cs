using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor.UIElements;
using UnityEngine;

public class JsonDictionaryConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>> where TKey : notnull {
	public override Dictionary<TKey, TValue> ReadJson(JsonReader reader, Type objectType, Dictionary<TKey, TValue> existingValue, bool hasExistingValue, JsonSerializer serializer) {
		var result = new Dictionary<TKey, TValue>();
		var array = JArray.Load(reader);
		foreach (var item in array) {
			var key = item["key"]!.ToObject<TKey>(serializer);
			var value = item["value"]!.ToObject<TValue>(serializer);
			result[key] = value;
		}
		return result;
	}

	public override void WriteJson(JsonWriter writer, Dictionary<TKey, TValue> value, JsonSerializer serializer) {
		writer.WriteStartArray();
		foreach (var kvp in value) {
			writer.WriteStartObject();
			writer.WritePropertyName("key");
			serializer.Serialize(writer, kvp.Key, typeof(TKey));
			writer.WritePropertyName("value");
			serializer.Serialize(writer, kvp.Value, typeof(TValue));
			writer.WriteEndObject();
		}
		writer.WriteEndArray();
	}
}
