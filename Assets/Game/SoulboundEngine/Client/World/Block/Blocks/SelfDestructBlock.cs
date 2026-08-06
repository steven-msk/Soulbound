using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.World.Block {
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
