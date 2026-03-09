using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class PlaceableItem : Item, IPlaceableItem, IItemAction {
		public override string name => "Placeable Item";

		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public PlaceableItem() : base("placeableItem") {
		}

		bool IItemAction.ValidateTrigger(ItemActionTrigger trigger) {
			return trigger == ItemActionTrigger.LeftClick;
		}

		public BlockState GetBlockState(ItemStack itemStack) {
			return Blocks.movingTickingBlock.defaultState;
		}
	}
}
