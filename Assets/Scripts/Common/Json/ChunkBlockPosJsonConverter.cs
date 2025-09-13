using SoulboundBackend.Client.World.Chunk;
using System;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace SoulboundBackend.Common.Json {
	public sealed class ChunkBlockPosJsonConverter : JsonConverter<ChunkBlockPos> {
		public override ChunkBlockPos ReadJson(JsonReader reader, Type objectType, ChunkBlockPos existingValue, bool hasExistingValue, JsonSerializer serializer) {
			JArray array = JArray.Load(reader);
			return new ChunkBlockPos(array[0]!.Value<int>(), array[1]!.Value<int>(), array[2]!.Value<int>());
		}

		public override void WriteJson(JsonWriter writer, ChunkBlockPos value, JsonSerializer serializer) {
			writer.WriteStartArray();
			serializer.Serialize(writer, value.x);
			serializer.Serialize(writer, value.y);
			serializer.Serialize(writer, value.chunkX);
			writer.WriteEndArray();
		}
	}
}