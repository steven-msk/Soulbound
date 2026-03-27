using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.ItemSystem.View;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class ChargeableItem : Item, IItemInteractionListener {
		// might pull this up into IChargeableItem for the future

		public override string name => "Chargeable Item";

		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public ChargeableItem() : base("chargeableItem") {
		}

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.LeftHold
				|| trigger == InteractionTrigger.LeftClick
				|| trigger == InteractionTrigger.LeftRelease;
		}

		public bool CanExecute(ItemStack itemStack, in ItemInteraction ctx) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, in ItemInteraction ctx) {
			if (ctx.trigger == InteractionTrigger.LeftClick) {
				Logger.LogInfo("Start charge");
			} else if (ctx.trigger == InteractionTrigger.LeftHold) {
				Logger.LogInfo("Charging..........");
			} else {
				Logger.LogInfo("Released charge");
			}
			return true;
		}
	}
}
