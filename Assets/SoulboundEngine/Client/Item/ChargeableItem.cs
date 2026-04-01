using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class ChargeableItem : Item, IItemInteractionListener {
		// might pull this up into IChargeableItem for the future
		private static readonly Identifier identifier = new("chargeableItem");
		public override string name => "Chargeable Item";

		public ChargeableItem() : base(identifier) {
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

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData("bluething", itemStack);
		}
	}
}
