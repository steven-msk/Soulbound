namespace SoulboundEngine.World.Level {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Block.State;
	using System;

#nullable enable

	public interface IQueriableLevel {
		bool IsStateAtPosition(BlockPos blockPos, Predicate<BlockState> predicate);

		T? GetTileEntity<T>(BlockPos blockPos, TileEntityType<T> type) where T : TileEntity;

		BlockPos GetTopPosition(BlockPos blockPos);
	}
}
