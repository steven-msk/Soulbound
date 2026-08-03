using SoulboundEngine.Common;

namespace SoulboundEngine.Client.Item {
	[PROTOTYPICAL]
	public sealed class DebugPointerItem : Item {
		public DebugPointerItem(Settings settings) : base(settings) {
		}

		//public bool ValidateTrigger(InteractionTrigger trigger) {
		//	return trigger == InteractionTrigger.LeftClick;
		//}

		//public bool CanExecute(in ItemStack itemStack, in ItemInteraction ctx) {
		//	return true;
		//}

		//public bool TryExecute(ref ItemStack itemStack, in ItemInteraction ctx) {
		//	Logger.LogInfo("Pointer: {}", ctx.player.GetWorldPointerPos());
		//	return true;
		//}
	}
}
