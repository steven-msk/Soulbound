using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DefaultBlockStateBehavior : IBlockStateBehavior {
    public List<ItemStack> DroppedItemsUponBroken(BlockState blockState) {
        return new List<ItemStack>() {
            new ItemStack(BlockItem.FromBlock(blockState.block), 1)
        };
    }

    public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
    }
}
