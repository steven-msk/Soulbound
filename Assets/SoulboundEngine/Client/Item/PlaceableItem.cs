using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class PlaceableItem : Item, IPlaceableItem, IItemInteractionListener {
		public override string name => "Placeable Item";

		public BlockState GetBlockState(ItemStack itemStack) {
			return Blocks.movingTickingBlock.defaultState;
		}

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData("bluething", itemStack);
		}

	}
}
