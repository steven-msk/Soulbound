using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	[JsonConverter(typeof(BlockState.Serializer))]
	public class BlockState {
		public Block block { get; }
		private readonly BlockPropertyEntries properties;
		public readonly int stateHash;
		internal int stateID;	// assigned by registry

		internal BlockState(Block block, BlockPropertyEntries properties) {
			this.block = block ?? Blocks.air;
			this.properties = properties;
			stateHash = StateHasher.ComputeHash(block, properties);
		}

		public T Get<T>(BlockProperty<T> property) => properties.Get<T>(property);

		public override bool Equals(object obj) {
			return obj is BlockState other
				&& other.stateHash == this.stateHash;
		}

		public override int GetHashCode() => stateHash;

		public override string ToString() {
			return $"BlockState[" +
				$"block:{block.name}, " +
				$"properties:{properties}]";
		}


		public sealed class Serializer : JsonConverter<BlockState> {
			private static readonly BlockState fallback = Blocks.air.defaultState;

			public override BlockState? ReadJson(JsonReader reader, Type objectType, BlockState? existingValue, bool hasExistingValue, JsonSerializer serializer) {
				if (reader.TokenType == JsonToken.Null) {
					return fallback;
				}
				if (reader.TokenType != JsonToken.Integer) {
					Logger.LogError("Invalid json token for block state: {}", reader.TokenType);
					return fallback;
				}

				int hash = Convert.ToInt32(reader.Value);
				return ToState(hash);
			}

			public override void WriteJson(JsonWriter writer, BlockState? value, JsonSerializer serializer) {
				if (value == null) {
					writer.WriteNull();
					return;
				}

				writer.WriteValue(value.stateHash);
			}

			public static BlockState ToState(int hash) {
				if (BlockStateRegistry.TryGetByHash(hash, out var state)) {
					return state;
				}
				Logger.LogError("Unknown state hash: {}", hash);
				return fallback;
			}
		}

	}
}
