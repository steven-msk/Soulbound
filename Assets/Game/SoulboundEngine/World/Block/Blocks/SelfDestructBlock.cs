namespace SoulboundEngine.World.Block {
	using SoulboundEngine.Common;
	using SoulboundEngine.World.Block.Entity;
	using SoulboundEngine.World.Block.State;

	[PROTOTYPICAL]
	public sealed class SelfDestructBlock : Block, ITileEntityProvider {
		public SelfDestructBlock(AbstractBlock.Settings settings) 
			: base(settings) {
		}

		public TileEntity CreateTileEntity(BlockPos pos, BlockState state) {
			return SelfDestructEntity.Create(pos, state);
		}
	}
}
