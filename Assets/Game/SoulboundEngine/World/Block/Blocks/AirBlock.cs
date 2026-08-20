using SoulboundEngine.World.Block.State;

namespace SoulboundEngine.World.Block {
	public class AirBlock : Block {
		public AirBlock(Settings settings) 
			: base(settings) {
		}

		protected override BlockShape GetShape(BlockState state, BlockPos blockPos, Level.Level level) {
			return BlockShape.EMPTY;
		}
	}
}
