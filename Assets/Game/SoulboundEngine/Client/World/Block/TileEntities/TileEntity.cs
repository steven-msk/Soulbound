using SoulboundEngine.Client.World.LevelDomain;

namespace SoulboundEngine.Client.World.BlockSystem.TileEntities {
	public abstract class TileEntity {
		protected readonly Level level;
		public readonly BlockPos blockPos;
		protected readonly TileEntityType tileEntityType;

		public TileEntity(TileEntityType tileEntityType, Level level, BlockPos blockPos) {
			this.tileEntityType = tileEntityType;
			this.level = level;
			this.blockPos = blockPos;
		}

		public virtual void OnDispose() { }
	}
}
