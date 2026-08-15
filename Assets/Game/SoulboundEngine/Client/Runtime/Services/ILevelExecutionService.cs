using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Entity;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Runtime.Services {
	public interface ILevelExecutionService {
		void SetBlockState(BlockPos blockPos, BlockState? blockState);
		void SpawnEntity(EntityDescriptor descriptor, Vector2 pos);
	}
}
