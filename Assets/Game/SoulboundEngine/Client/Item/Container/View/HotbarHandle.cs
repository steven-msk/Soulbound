namespace SoulboundEngine.Client.ItemSystem.Container.View {
	public class HotbarHandle : InventoryHandle {
		private Hotbar hotbar;
		private HotbarSlotHandle[] handles;
		private bool fadedLayout;

		public void Init(Hotbar hotbar, IItemContainerScope scope, HotbarSlotHandle[] handles) {
			base.Init(hotbar, scope);
			hotbar.mainSlotChanged += OnMainSlotChanged;
			this.hotbar = hotbar;
			this.handles = handles;
		}

		public void ToggleFadedLayout() {
			fadedLayout = !fadedLayout;
			foreach (var handle in handles) handle.ApplyFadedLayout(fadedLayout);
			handles[hotbar.GetMainSlotIndex()].SetMainSlotLayout();
		}

		private void OnMainSlotChanged(int oldIndex, int newIndex) {
			handles[oldIndex].RemoveMainSlotLayout();
			handles[newIndex].SetMainSlotLayout();
		}

		private void OnDestroy() => hotbar.mainSlotChanged -= OnMainSlotChanged;
	}
}
