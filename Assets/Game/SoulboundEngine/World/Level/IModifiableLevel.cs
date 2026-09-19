namespace SoulboundEngine.World.Level {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Entity;
	using System;

	public interface IModifiableLevel {
		bool SetBlockState(BlockPos blockPos, BlockState blockState);

		bool RemoveBlock(BlockPos blockPos);

		bool AddNewEntity(Entity entity);

		[Obsolete] void RemoveEntity(Entity entity);
	}
}
