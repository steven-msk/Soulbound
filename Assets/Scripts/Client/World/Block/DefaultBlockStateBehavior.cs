using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DefaultBlockStateBehavior : IBlockStateBehavior {
    public string Description => "Drops a single item on break, does not update neighbors";

    public List<ItemStack> GetDrops(BlockState blockState) {
        return new List<ItemStack>() {
            new ItemStack(BlockItem.FromBlock(blockState.block), 1)
        };
    }

    public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
    }
}
