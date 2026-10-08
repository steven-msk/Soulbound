namespace SoulboundEngine.World.Level {
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.Entity;

	public interface IRegistryLevelView : ILevelView, IModifiableQueriableLevel, IEntityView {
		T IQueriableLevel.GetTileEntity<T>(BlockPos blockPos, TileEntityType<T> type) where T : class {
			return ((ILevelView)this).GetTileEntity(blockPos, type);
		}

		BlockPos IQueriableLevel.GetTopPosition(BlockPos blockPos) {
			return ((ILevelView)this).GetTopPosition(blockPos);
		}
	}
}
