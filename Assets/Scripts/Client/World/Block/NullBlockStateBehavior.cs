using SoulboundBackend.Client.ItemSystem;
using System.Collections.Generic;

namespace SoulboundBackend.Client.World.BlockSystem {
	public class NullBlockStateBehavior : IBlockStateBehavior {
		public string Description => "No drops, no neighbor updates";

		public List<ItemStack> GetDrops(BlockState blockState, BreakSource source) => new List<ItemStack>();

		public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
		}

		public void OnPlace(BlockPos blockPos, BlockState blockState) {
		}
	}
}
