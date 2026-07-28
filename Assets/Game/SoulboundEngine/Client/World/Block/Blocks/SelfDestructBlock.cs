using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Block.TileEntity;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.World.Block {
	[PROTOTYPICAL]
	public sealed class SelfDestructBlock : Block {
		public SelfDestructBlock(Settings settings) 
			: base(settings) {
		}

		public override bool HasTileEntity(Level.Level level, BlockPos blockPos, BlockState blockState) => true;

		public override TileEntity.TileEntity GetTileEntity(Level.Level level, BlockPos blockPos) {
			return new SelfDestructEntity(TileEntityTypes.SELF_DESTRUCT_BLOCK, level, blockPos);
		}
	}
}
