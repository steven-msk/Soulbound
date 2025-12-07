using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor.Rendering;
using UnityEngine;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	//[JsonConverter(typeof(BlockStateJsonConverter))]
	public sealed class BlockState {
		static readonly Logger logger = Logger.CreateInstance();
		public Block block { get; }
		private readonly BlockPropertyEntries properties;
		public int hash => this.GetHashCode();

		public BlockState(Block block, BlockPropertyEntries properties) {
			this.block = block ?? Blocks.air;
			this.properties = properties;
		}

		public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
			block.OnNeighborStateChanged(selfPos, neighborPos, oldState, newState);
		}

		public void DropOnBroken(BlockPos pos, BreakSource source) {
			Vector2 dropForce = new(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(2.5f, 3f));
			var drops = block.GetDrops(this, source);

			foreach (var stack in drops) {
				stack.Drop(pos.CenterAligned(), dropForce);
			}
		}

		public T Get<T>(BlockProperty<T> property) => properties.Get<T>(property);

		public BlockState With<T>(BlockProperty<T> property, T value) {
			if (!block.HasProperty(property)) {
				throw new InvalidOperationException($"Block {block.name} does not have property {property.name}");
			}

			if (EqualityComparer<T>.Default.Equals(Get(property), value)) {
				return this;
			}
			return new BlockState(block, properties.With(property, value));
		}

		public override bool Equals(object obj) {
			return obj is BlockState other
				&& other.hash == this.hash;
		}

		public override int GetHashCode() {
			return StateHasher.ComputeHash(block, properties);
		}

		public override string ToString() {
			return $"BlockState[" +
				$"block:{block.name}, " +
				$"properties:{properties}]";
		}

		//	[Obsolete]
		//	[JsonConverter(typeof(PersistencyInfo.PersistencyJsonConverter))]
		//	public record PersistencyInfo(BlockPropertyEntries properties) {
		//		public int hash => properties.GetHashCode();

		//		public BlockState ToBlockState(Block block) {
		//			return new BlockState(block, properties);
		//		}

		//		[Obsolete]
		//		public class PersistencyJsonConverter : JsonConverter<PersistencyInfo> {
		//			public override PersistencyInfo? ReadJson(JsonReader reader, Type objectType, PersistencyInfo? existingValue, bool hasExistingValue, JsonSerializer serializer) {
		//				if (reader.TokenType == JsonToken.Null) {
		//					return null;
		//				}
		//				JObject obj = JObject.Load(reader);
		//				var propertiesToken = obj["properties"] as JObject
		//					?? throw new JsonSerializationException("Missing 'properties' field in PersistencyInfo JSON.");
		//				var properties = new Dictionary<IBlockStateProperty, object>();

		//				foreach (var kvp in propertiesToken) {
		//					JToken valueToken = kvp.Value!;
		//					if (valueToken == null) {
		//						continue;
		//					}

		//					if (valueToken.Type == JTokenType.Object
		//							&& valueToken["value"] != null
		//							&& valueToken["type"] != null) {
		//						string typeName = valueToken["type"]!.Value<string>()!;
		//						Type valueType = Type.GetType(typeName)!;
		//						object value = valueToken["value"]!.ToObject(valueType, serializer)!;

		//						Type propertyType = typeof(BlockProperty<>).MakeGenericType(valueType);
		//						IBlockStateProperty property = (IBlockStateProperty)Activator.CreateInstance(propertyType, kvp.Key);

		//						properties[property] = value;
		//					} else {
		//						object? primitiveValue = valueToken.ToObject<object>(serializer);
		//						Type valueType = primitiveValue?.GetType() ?? typeof(object);

		//						Type propertyType = typeof(BlockProperty<>).MakeGenericType(valueType);
		//						IBlockStateProperty property = (IBlockStateProperty)Activator.CreateInstance(propertyType, kvp.Key);

		//						properties[property] = primitiveValue!; 
		//					}
		//				}

		//				//return new PersistencyInfo(new BlockStateProperties(properties));
		//				return default;
		//			}

		//			public override void WriteJson(JsonWriter writer, PersistencyInfo? value, JsonSerializer serializer) {
		//				if (value == null) {
		//					writer.WriteNull();
		//					return;
		//				}

		//				writer.WriteStartObject();
		//				writer.WritePropertyName("hash");
		//				serializer.Serialize(writer, value.hash);
		//				writer.WritePropertyName("properties");
		//				writer.WriteStartObject();
		//				//foreach (var kvp in value.properties) {
		//				//	writer.WritePropertyName(kvp.Key.name);
		//				//	serializer.Serialize(writer, kvp.Value);
		//				//}
		//				writer.WriteEndObject();
		//				writer.WriteEndObject();
		//			}
		//		}
		//	}

		//	[Obsolete]
		//	public static class Serializer {
		//		[Obsolete]
		//		public static BlockState? Deserialize(JArray dataArray) {
		//			if (dataArray.Count < 2) {
		//				return null;
		//			}

		//			int blockID = dataArray[0]!.Value<int>();
		//			int stateHash = dataArray[1]!.Value<int>();

		//			Block block = Blocks.ByHashedID(blockID);
		//			if (block == null) {
		//				return Blocks.air.defaultState;
		//			}

		//			if (block.TryGetState(stateHash, out var state)) {
		//				return state;
		//			}

		//			logger.LogWarning($"Unknown state hash {stateHash} for block '{block.name}'");
		//			return block.defaultState;
		//		}

		//		[Obsolete]
		//		public static JArray Serialize(BlockState state) {
		//			int blockID = state.block.hashedID;
		//			//int stateHash = state.block.ComputeHash(state.properties_obsolete);

		//			//return new JArray { blockID, stateHash };
		//			return new();
		//		}
		//	}

		//	public sealed class BlockStateJsonConverter : JsonConverter<BlockState> {
		//		public override BlockState? ReadJson(JsonReader reader, Type objectType, BlockState? existingValue, bool hasExistingValue, JsonSerializer serializer) {
		//			if (reader.TokenType == JsonToken.Null) {
		//				return null;
		//			}

		//			var array = JArray.Load(reader);
		//			return Serializer.Deserialize(array);
		//		}

		//		public override void WriteJson(JsonWriter writer, BlockState? value, JsonSerializer serializer) {
		//			if (value == null) {
		//				writer.WriteNull();
		//				return;
		//			}

		//			serializer.Serialize(writer, Serializer.Serialize(value));
		//		}
		//	}
	}
}
