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
	public sealed class ChargeableItem : Item, IItemAction {
		// might pull this up into IChargeableItem for the future

		public override string name => "Chargeable Item";

		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public ChargeableItem() : base("chargeableItem") {
		}

		public bool ValidateTrigger(ItemActionTrigger trigger) {
			return trigger == ItemActionTrigger.LeftHold
				|| trigger == ItemActionTrigger.LeftClick
				|| trigger == ItemActionTrigger.LeftRelease;
		}

		public bool CanExecute(ItemStack itemStack, ItemActionContext ctx) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, ItemActionContext ctx) {
			if (ctx.trigger == ItemActionTrigger.LeftClick) {
				Logger.LogInfo("Start charge");
			} else if (ctx.trigger == ItemActionTrigger.LeftHold) {
				Logger.LogInfo("Charging..........");
			} else {
				Logger.LogInfo("Released charge");
			}
			return true;
		}
	}
}
