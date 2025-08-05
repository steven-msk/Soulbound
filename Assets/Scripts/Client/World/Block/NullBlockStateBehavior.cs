using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NullBlockStateBehavior : IBlockStateBehavior {
    public string Description => "No drops, no neighbor updates";

    public List<ItemStack> GetDrops(BlockState blockState, BreakSource source) => new List<ItemStack>();

    public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
    }

    public void OnPlace(BlockPos blockPos, BlockState blockState) {
    }
}
