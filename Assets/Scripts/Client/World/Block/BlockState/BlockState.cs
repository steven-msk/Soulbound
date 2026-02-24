using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	[JsonConverter(typeof(BlockState.Serializer))]
	public sealed class BlockState {
		public Block block { get; }
		private readonly BlockPropertyEntries properties;
		public int stateHash => StateHasher.ComputeHash(block, properties);

		public BlockState(Block block, BlockPropertyEntries properties) {
			this.block = block ?? Blocks.air;
			this.properties = properties;
		}

		public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState? oldState, BlockState? newState) {
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
				if (BlockStateRegistry.TryGet(hash, out var state)) {
					return state;
				}
				Logger.LogError("Unknown state hash: {}", hash);
				return fallback;
			}
		}

	}
}
