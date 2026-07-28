using SoulboundEngine.Client.World.Level;

namespace SoulboundEngine.Client.World.Block.TileEntity {
	public abstract class TileEntity {
		protected readonly Level.Level level;
		public readonly BlockPos blockPos;
		protected readonly TileEntityType tileEntityType;

		public TileEntity(TileEntityType tileEntityType, Level.Level level, BlockPos blockPos) {
			this.tileEntityType = tileEntityType;
			this.level = level;
			this.blockPos = blockPos;
		}

		public virtual void OnDispose() { }

		public TileEntityType GetTileEntityType() => tileEntityType;
	}
}
