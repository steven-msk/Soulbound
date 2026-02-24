using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SoulboundBackend.Client.World.BlockSystem {
    [JsonConverter(typeof(BlockPropertyJsonConverter))]
    public interface IBlockStateProperty {
        string name { get; }
        Type valueType { get; }
    }

    class BlockPropertyJsonConverter : JsonConverter<IBlockStateProperty> {
        public override IBlockStateProperty ReadJson(JsonReader reader, Type objectType, IBlockStateProperty existingValue, bool hasExistingValue, JsonSerializer serializer) {
            JObject obj = JObject.Load(reader);
            string name = obj["name"]!.ToObject<string>();
            string typeName = obj["type"]!.ToObject<string>();

            Type valueType = Type.GetType(typeName)!;
            Type constructed = typeof(BlockProperty<>).MakeGenericType(valueType);

            return (IBlockStateProperty)Activator.CreateInstance(constructed, name);
        }

        public override void WriteJson(JsonWriter writer, IBlockStateProperty value, JsonSerializer serializer) {
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            writer.WriteValue(value.name);

            writer.WritePropertyName("type");
            writer.WriteValue(value.GetType().GenericTypeArguments[0].AssemblyQualifiedName);
            writer.WriteEndObject();
        }
    }
}
