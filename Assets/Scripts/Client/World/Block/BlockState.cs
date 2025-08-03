using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockState {
    // subject to change in the future
    // for now, it is just a reference to the block type
    // this is a placeholder for future properties like metadata, state, etc.

    public Block block { get; private set; }
    public Dictionary<string, object> properties { get; private set; } = new();
    // might be replaced with a type-safe data structure in the future

    public BlockState(Block block) {
        this.block = block ?? throw new ArgumentNullException(nameof(block));
    }

    public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
        block.StateBehavior.OnNeighborStateChanged(selfPos, neighborPos, oldState, newState);
    }

    // TODO: implement block items for stone, wood, and dirt blocks

    public void DropOnBroken(BlockPos pos, BreakSource source) {
        if (block.BlockItemReference != null) {
            List<ItemStack> itemsDropped = block.StateBehavior.GetDrops(this, source);
            Vector2 dropForce = block.StateBehavior.dropForce;
            itemsDropped.ForEach(itemStack => itemStack.Drop(pos.CenterAligned(), dropForce));
        }
    }

    // temporary operator overloads for easy comparison
    // these will be replaced with more robust methods in the future
    // they will also check equality between properties of the block state in the future
    public static bool operator ==(BlockState state1, BlockState state2) {
        return state1 is not null && state2 is not null && state1.block == state2.block;
    }

    public static bool operator !=(BlockState state1, BlockState state2) => !(state1 == state2);

    public override bool Equals(object obj) {
        if (obj is BlockState other) {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode() {
        return block != null ? block.GetHashCode() : 0;
    }

    // TODO: override BlockState ToString() for better debugging
}
