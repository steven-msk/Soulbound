using SoulboundBackend.Client.World.LevelDomain;

namespace SoulboundBackend.Client.World.BlockSystem.TileEntities {
	public abstract class TileEntity {
		protected readonly Level level;
		public readonly BlockPos blockPos;

		public TileEntity(Level level, BlockPos blockPos) {
			this.level = level;
			this.blockPos = blockPos;
		}

		public virtual void OnDispose() { }
	}
}
