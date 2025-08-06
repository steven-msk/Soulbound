using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LeafBlockStateBehavior : IBlockStateBehavior {
    public string Description => "Drops only on player break source, no neighbor updates (yet)";
    public List<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
        if (source == BreakSource.Player) {
           return new List<ItemStack>() { new ItemStack(blockState.block.itemReference, 1) };
        }
        return new List<ItemStack>() { };
    }
    public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
    }

    public void OnPlace(BlockPos blockPos, BlockState blockState) {
    }
}
