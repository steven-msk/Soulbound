using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.ItemSystem.View;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class TeleportPlayerItem : Item, IItemInteractionListener {
		public override string name => "Move Player Item";
		public override int fullStackSize => 1;

		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("bluething"));

		public TeleportPlayerItem() : base("teleportPlayerItem") {
		}

		public bool ValidateTrigger(InteractionTrigger trigger) {
			return trigger == InteractionTrigger.LeftClick;
		}

		public bool CanExecute(ItemStack itemStack, in ItemInteraction ctx) {
			return true;
		}

		public bool TryExecute(ItemStack itemStack, in ItemInteraction ctx) {
			ctx.player.SetPos(ctx.player.GetWorldPointerPos());
			return true;
		}
	}
}
