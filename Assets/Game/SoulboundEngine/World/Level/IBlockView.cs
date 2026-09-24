namespace SoulboundEngine.World.Level {
	using SoulboundEngine.World;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Physics;
	using System.Collections.Generic;
	using System.Linq;

#nullable enable

	public interface IBlockView : IHeightLimitView {
		TileEntity? GetTileEntity(BlockPos blockPos);

		BlockState GetBlockState(BlockPos blockPos);

		public virtual T? GetTileEntity<T>(BlockPos blockPos, TileEntityType<T> type) where T : TileEntity {
			TileEntity? tileEntity = this.GetTileEntity(blockPos);
			return tileEntity != null && tileEntity.GetTileEntityType() == type ? (T)tileEntity : null;
		}

		public virtual IEnumerable<BlockState> GetBlockStates(AABB box) {
			return BlockPos.Inside(box).Select(this.GetBlockState);
		}
	}
}
