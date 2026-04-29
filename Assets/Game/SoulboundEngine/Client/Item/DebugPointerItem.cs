using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class DebugPointerItem : Item, IItemInteractionListener {
		public DebugPointerItem(Settings settings) : base(settings) {
		}

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.LeftClick;
		}

		public bool CanExecute(ItemStack itemStack, in ItemInteraction ctx) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, in ItemInteraction ctx) {
			Logger.LogInfo("Pointer: {}", ctx.player.GetWorldPointerPos());
			return true;
		}

	}
}
