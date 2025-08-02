using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BlockState {
    // subject to change in the future
    // for now, it is just a reference to the block type
    // this is a placeholder for future properties like metadata, state, etc.

    public Block block { get; private set; }

    public BlockState(Block block) {
        this.block = block ?? throw new ArgumentNullException(nameof(block));
    }

    public void OnNeighborChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
        // This method needs to be flexible to handle different block types
        // For now, we will just log the change
        //Debug.Log($"Block at {selfPos} changed neighbor at {neighborPos} from {oldState.block.name} to {newState.block.name}");
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
