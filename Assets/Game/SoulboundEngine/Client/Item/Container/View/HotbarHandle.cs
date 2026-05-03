namespace SoulboundEngine.Client.ItemSystem.Container.View {
	public class HotbarHandle : InventoryHandle {
		private Inventory inventory;
		private HotbarSlotHandle[] handles;
		private bool fadedLayout;

		public void Init(Inventory inventory, IItemContainerScope scope, HotbarSlotHandle[] handles) {
			base.Init(inventory, scope);
			inventory.mainSlotChanged += this.OnMainSlotChanged;
			this.inventory = inventory;
			this.handles = handles;
		}

		public void ToggleFadedLayout() {
			this.fadedLayout = !this.fadedLayout;
			foreach (var handle in this.handles) handle.ApplyFadedLayout(this.fadedLayout);
			this.handles[this.inventory.GetMainSlot()].SetMainSlotLayout();
		}

		private void OnMainSlotChanged(int oldIndex, int newIndex) {
			this.handles[oldIndex].RemoveMainSlotLayout();
			this.handles[newIndex].SetMainSlotLayout();
		}

		private void OnDestroy() => this.inventory.mainSlotChanged -= this.OnMainSlotChanged;
	}
}
