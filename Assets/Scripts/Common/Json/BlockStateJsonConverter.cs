using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;

public sealed class BlockStateJsonConverter : JsonConverter<BlockState> {
	public override BlockState? ReadJson(JsonReader reader, Type objectType, BlockState? existingValue, bool hasExistingValue, JsonSerializer serializer) {
		if (reader.TokenType == JsonToken.Null) {
			return null;
		}

		int blockID = Convert.ToInt32(reader.Value);
		return Blocks.ByID(blockID).defaultState;
	}

	public override void WriteJson(JsonWriter writer, BlockState? value, JsonSerializer serializer) {
		serializer.Serialize(writer, value?.block.id);
	}
}
