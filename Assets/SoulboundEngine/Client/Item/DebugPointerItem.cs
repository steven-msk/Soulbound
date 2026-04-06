using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class DebugPointerItem : Item, IItemInteractionListener {
		private static readonly Identifier identifier = new("debug_pointer");
		public override string name => "Debug Pointer";
		public override int fullStackSize => 1;

		public DebugPointerItem() : base(identifier) {
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

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData("debugPointer", itemStack);
		}
	}
}
