using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class PlaceableItem : Item, IPlaceableItem, IItemInteractionListener {
		public PlaceableItem(Settings settings) : base(settings) {
		}

		public BlockState GetBlockState(ItemStack itemStack) {
			return Blocks.movingTickingBlock.DefaultState;
		}

	}
}
