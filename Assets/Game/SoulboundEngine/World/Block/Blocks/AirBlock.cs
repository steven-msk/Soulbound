namespace SoulboundEngine.World.Block {
	using SoulboundEngine.Common.Collection;
	using SoulboundEngine.Item;
	using SoulboundEngine.Loot.Context;
	using SoulboundEngine.World.Block.State;
	using System.Collections.Generic;

	public class AirBlock : Block {
		public AirBlock(Settings settings) 
			: base(settings) {
		}

		protected override BlockShape GetShape(BlockState state, BlockPos blockPos, Level.Level level) {
			return BlockShape.EMPTY;
		}

		protected override IReadOnlyList<ItemStack> GetDrops(BlockState blockState, LootWorldContext.Builder context) {
			return Collections.EmptyList<ItemStack>();
		}
	}
}
