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
        Debug.Log($"Block at {selfPos} changed neighbor at {neighborPos} from {oldState.block.name} to {newState.block.name}");
    }
}
