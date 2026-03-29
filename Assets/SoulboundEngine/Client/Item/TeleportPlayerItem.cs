using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class TeleportPlayerItem : Item, IItemInteractionListener {
		public override string name => "Move Player Item";
		public override int fullStackSize => 1;

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

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData("bluething", itemStack);
		}
	}
}
