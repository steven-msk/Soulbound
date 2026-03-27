using SoulboundEngine.Client.ItemSystem.View;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class PlaceableItem : Item, IPlaceableItem, IItemInteractionListener {
		public override string name => "Placeable Item";

		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public PlaceableItem() : base("placeableItem") {
		}

		public BlockState GetBlockState(ItemStack itemStack) {
			return Blocks.movingTickingBlock.defaultState;
		}
	}
}
