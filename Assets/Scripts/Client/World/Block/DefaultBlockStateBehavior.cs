using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;

namespace SoulboundBackend.Client.World.BlockSystem {
	public class DefaultBlockStateBehavior : IBlockStateBehavior {
		public string Description => "Drops a single item on break, does not update neighbors";

		public List<ItemStack> GetDrops(BlockState blockState, BreakSource source) {
			return new List<ItemStack>() {
				new ItemStack(blockState.block.itemReference, 1)
			};
		}

		public void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState) {
		}

		public void OnPlace(BlockPos blockPos, BlockState blockState) {
		}
	}
}
