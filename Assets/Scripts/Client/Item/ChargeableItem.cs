using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class ChargeableItem : Item, IItemInteractionListener {
		// might pull this up into IChargeableItem for the future

		public override string name => "Chargeable Item";

		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public ChargeableItem() : base("chargeableItem") {
		}

		public bool ValidateTrigger(ItemInteractionTrigger trigger) {
			return trigger == ItemInteractionTrigger.LeftHold
				|| trigger == ItemInteractionTrigger.LeftClick
				|| trigger == ItemInteractionTrigger.LeftRelease;
		}

		public bool CanExecute(ItemStack itemStack, ItemInteraction ctx) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, ItemInteraction ctx) {
			if (ctx.trigger == ItemInteractionTrigger.LeftClick) {
				Logger.LogInfo("Start charge");
			} else if (ctx.trigger == ItemInteractionTrigger.LeftHold) {
				Logger.LogInfo("Charging..........");
			} else {
				Logger.LogInfo("Released charge");
			}
			return true;
		}
	}
}
