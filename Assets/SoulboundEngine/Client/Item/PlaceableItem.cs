using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class PlaceableItem : Item, IPlaceableItem, IItemInteractionListener {
		private static readonly Identifier identifier = new("placeableItem");
		public override string name => "Placeable Item";

		public PlaceableItem() : base(identifier) {
		}

		public BlockState GetBlockState(ItemStack itemStack) {
			return Blocks.movingTickingBlock.defaultState;
		}

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData("bluething", itemStack);
		}

	}
}
