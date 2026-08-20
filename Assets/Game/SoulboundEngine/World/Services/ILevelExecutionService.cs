using SoulboundEngine.Common.Math;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Entity;

#nullable enable

namespace SoulboundEngine.World.Services {
	public interface ILevelExecutionService {
		void SetBlockState(BlockPos blockPos, BlockState? blockState);
		void SpawnEntity(EntityDescriptor descriptor, Vec2d pos);
	}
}
