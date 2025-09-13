using System.Collections.Generic;

public class NullBlockStateBehavior : IBlockStateBehavior {
    public string Description => "No drops, no neighbor updates";

    public List<ItemStack> GetDrops(BlockState blockState, BreakSource source) => new List<ItemStack>();

    public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
    }

    public void OnPlace(BlockPos blockPos, BlockState blockState) {
    }
}
