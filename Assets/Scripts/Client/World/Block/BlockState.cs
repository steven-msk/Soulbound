using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

#nullable enable

[JsonConverter(typeof(BlockStateJsonConverter))]
public class BlockState {
    public Block block { get; private set; }

    // might be replaced with a type-safe data structure in the future
    public Dictionary<string, object> properties { get; private set; } = new();

    public IBlockStateBehavior stateBehavior { get; private set; }

    public BlockState(Block block, Dictionary<string, object>? properties, IBlockStateBehavior stateBehavior) {
        this.block = block ?? Blocks.air;
        this.properties = properties ?? new Dictionary<string, object>();
        this.stateBehavior = stateBehavior;
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

    public static bool operator ==(BlockState? state1, BlockState? state2) {
        return state1 is not null && state2 is not null && state1.block == state2.block;
    }

    public static bool operator !=(BlockState? state1, BlockState? state2) => !(state1! == state2!);

    public override bool Equals(object obj) {
        if (obj is BlockState other) {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode() => block.GetHashCode();

    public override string ToString() {
        return $"BlockState[{block.name}]";
    }

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
}
