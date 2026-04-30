using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.World.BlockSystem {
	[PROTOTYPICAL]
	public sealed class SelfDestructBlock : Block {
		public SelfDestructBlock(Settings settings) 
			: base(settings) {
		}

		public override bool HasTileEntity(Level level, BlockPos blockPos, BlockState blockState) => true;

		public override TileEntity GetTileEntity(Level level, BlockPos blockPos) {
			return new SelfDestructEntity(level, blockPos);
		}
	}
}
