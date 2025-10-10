using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World.BlockSystem;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SoulboundBackend.Client.World {
	public interface IBlockStateBehavior {
		// all of these method implementations are subject to change in the future

		virtual Vector2 dropForce => new(Random.Range(-1f, 1f), Random.Range(2.5f, 3f));
		List<ItemStack> GetDrops(BlockState blockState, BreakSource source);
		void OnNeighborStateChanged(BlockPos selfPos, BlockPos neighborPos, BlockState oldState, BlockState newState);
		void OnPlace(BlockPos blockPos, BlockState blockState);
    }
}
