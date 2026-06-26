using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.Item {
	[PROTOTYPICAL]
	public sealed class TeleportPlayerItem : Item, IInteractableItem {
		public TeleportPlayerItem(Settings settings) : base(settings) {
		}

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.LeftClick;
		}

		public bool CanExecute(in ItemStack itemStack, in ItemInteraction ctx) {
			return true;
		}

		public bool TryExecute(ref ItemStack itemStack, in ItemInteraction ctx) {
			ctx.player.SetPosition(ctx.player.GetWorldPointerPos());
			return true;
		}

	}
}
