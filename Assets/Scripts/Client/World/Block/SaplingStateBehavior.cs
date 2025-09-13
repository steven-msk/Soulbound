using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Structure.Templates;
using SoulboundBackend.Core;
using System.Collections.Generic;

namespace SoulboundBackend.Client.World.BlockSystem {
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
}
