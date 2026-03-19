using SoulboundBackend.Client.ItemSystem.View;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
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
