
namespace SoulboundEngine.Client.ItemSystem.Container {
	public sealed class ReleaseTransitInEmptySlot : SingleSlotOperation {
		public ReleaseTransitInEmptySlot(IItemContainer container, int slotIndex, IItemContainerScope scope)
			: base(container, slotIndex, scope) {
		}

		public override bool CanExecute() {
			return !slot.HasStack() && scope.HasTransitStack();
		}

		public override bool Execute() {
			if (!CanExecute()) return false;

			slot.SetStack(scope.GetTransitStack());
			scope.SetTransitStack(null);
			return true;
		}
	}
}
