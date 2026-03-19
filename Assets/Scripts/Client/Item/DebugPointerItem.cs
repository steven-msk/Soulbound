using SoulboundBackend.Client.Interaction;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;
using SoulboundBackend.Client.Debug.Logging;
using SoulboundBackend.Client.ItemSystem.View;

namespace SoulboundBackend.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class DebugPointerItem : Item, IItemInteractionListener {
		public override string name => "Debug Pointer";
		public override int fullStackSize => 1;
		public override ItemAspect aspect => ItemAspect.Simple(new AssetKey("debugPointer"));

		public DebugPointerItem() : base("debugPointer") {
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
