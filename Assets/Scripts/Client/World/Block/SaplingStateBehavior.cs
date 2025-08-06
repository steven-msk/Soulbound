using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SaplingStateBehavior : IBlockStateBehavior {
    public string Description => "Tree.";

    public List<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
        return new List<ItemStack>() { };
    }

    public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
    }

    // CanPlaceAt?

    public void OnPlace(BlockPos blockPos, BlockState blockState) {
        Level level = GameManager.instance.Level;
        level.ForcePlaceStructure(ChunkBlockPos.FromBlockPos(blockPos), TreeStructure.instance);
    }
}
