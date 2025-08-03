using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IBlockStateBehavior {
    // all of these method implementations are subject to change in the future

    string Description { get; }      // solely for inspector display, not used in-game

    List<ItemStack> GetDrops(BlockState blockState, BreakSource source);

    void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState);
}
