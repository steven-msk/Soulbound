using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor.Rendering;
using UnityEngine;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	[JsonConverter(typeof(BlockStateJsonConverter))]
	public sealed class BlockState {
		static readonly Logger logger = Logger.CreateInstance();
		public Block block { get; }
		public BlockStateProperties properties { get; }
		public IBlockStateBehavior stateBehavior { get; }

		public BlockState(Block block, Dictionary<IBlockStateProperty, object>? properties, IBlockStateBehavior stateBehavior) {
			this.block = block ?? Blocks.air;
			this.stateBehavior = stateBehavior;

			properties ??= new Dictionary<IBlockStateProperty, object>();
			var unspecified = block!.propertyDefinitions.Where(p => !properties.ContainsKey(p));
			foreach (var property in unspecified) {
				properties.Add(property, block.GetDefaultValueOfProperty(property));
			}

			this.properties = new BlockStateProperties(properties);
		}

		public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
			stateBehavior.OnNeighborStateChanged(selfPos, neighborPos, oldState, newState);
		}

		public void DropOnBroken(BlockPos pos, BreakSource source) {
			if (block != Blocks.air) {
				List<ItemStack> itemsDropped = stateBehavior.GetDrops(this, source);
				Vector2 dropForce = stateBehavior.dropForce;
				itemsDropped.ForEach(itemStack => itemStack.Drop(pos.CenterAligned(), dropForce));
			}
		}

		public void OnPlace(BlockPos blockPos) => stateBehavior.OnPlace(blockPos, this);

		public T Get<T>(BlockProperty<T> property) => (T)properties[property];

		public object Get(IBlockStateProperty property) => properties[property];

		public object Get(string property) {
			var target = properties.Keys.FirstOrDefault(p => p.name == property);
			return properties[target] ?? default!;
		}

        public T Get<T>(string property) => (T)Get(property);

        public BlockState With<T>(BlockProperty<T> property, T value) {
			if (!block.HasProperty(property)) {
				throw new InvalidOperationException($"Block {block.name} does not have property {property.name}");
			}

			if (EqualityComparer<T>.Default.Equals(Get(property), value)) {
				return this;
			}

			var newProperties = new BlockStateProperties(new Dictionary<IBlockStateProperty, object>() {
				[property] = value!
			});

			return block.GetStateFor(newProperties);
		}

		public static bool operator ==(BlockState? state1, BlockState? state2) {
			return state1 is not null && state2 is not null 
				&& state1.block == state2.block
				&& state1.properties == state2.properties;
		}

		public static bool operator !=(BlockState? state1, BlockState? state2) => !(state1! == state2!);

		public override bool Equals(object obj) {
			if (obj is BlockState other) {
				return this == other;
			}
			return false;
		}

		public override int GetHashCode() => properties.GetHashCode();

		public override string ToString() {
			return $"BlockState[{block.name}:{properties}]";
		}

		public static class Serializer {
			public static BlockState? Deserialize(JArray dataArray) {
				if (dataArray.Count < 2) {
					return null;
				}

				int blockID = dataArray[0]!.Value<int>();
				int stateHash = dataArray[1]!.Value<int>();

				Block block = Blocks.ByHashedID(blockID);
				if (block == null) {
					return Blocks.air.defaultState;
				}

				if (block.TryGetStateByHash(stateHash, out var state)) {
					return state;
				}

				logger.LogWarning(null, $"Unknown state hash {stateHash} for block '{block.name}'");
				return block.defaultState;
			}

			public static JArray Serialize(BlockState state) {
				int blockID = state.block.hashedID;
				int stateHash = state.block.ComputeHash(state.properties);

				return new JArray { blockID, stateHash };
			}
		}

		public sealed class BlockStateJsonConverter : JsonConverter<BlockState> {
			public override BlockState? ReadJson(JsonReader reader, Type objectType, BlockState? existingValue, bool hasExistingValue, JsonSerializer serializer) {
				if (reader.TokenType == JsonToken.Null) {
					return null;
				}

				var array = JArray.Load(reader);
				return Serializer.Deserialize(array);
			}

			public override void WriteJson(JsonWriter writer, BlockState? value, JsonSerializer serializer) {
				if (value == null) {
					writer.WriteNull();
					return;
				}

				serializer.Serialize(writer, Serializer.Serialize(value));
			}
		}
	}
}
