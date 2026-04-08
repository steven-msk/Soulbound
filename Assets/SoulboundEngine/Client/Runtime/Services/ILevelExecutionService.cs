using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.EntitySystem;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Runtime.Services {
	public interface ILevelExecutionService {
		void SetBlockState(BlockPos blockPos, BlockState? blockState);
		void SpawnEntity(EntityDescriptor descriptor, Vector2 pos);
	}
}
