using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;

namespace SoulboundEngine.Client.ItemSystem {
	public class BlockItem : Item, IPlaceableItem {
		private readonly Block block;

		public BlockItem(Block block, Settings settings) 
			: base(settings) {
			this.block = block;
		}

		public Block GetBlock() => this.block;

		public BlockState GetBlockState(ItemStack itemStack) {
			return this.block.DefaultState;
		}
	}
}
