namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Render.Item;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using SoulboundEngine.World.Player;
	using UnityEngine.UIElements;

	public sealed class WorldScreen : UXMLScreen {
		private static readonly UXMLBinding<VisualElement> HOTBAR_ELEMENT = new("soulbound:hotbar/hotbar");
		private readonly ItemRenderManager itemRenderManager;
		private readonly PlayerInventory playerInventory;
		private UXMLHotbarSlotDisplay[] hotbarDisplays;
		private VisualElement hotbarRoot;

		public WorldScreen(PlayerInventory playerInventory, ItemRenderManager itemRenderManager)
			: base(AssetManager.Resolve<VisualTreeAsset>(new AssetKey("WorldScreen"))) {
			this.playerInventory = playerInventory;
			this.itemRenderManager = itemRenderManager;
		}

		public override bool CloseOnEsc => false;

		protected override void OnBind(VisualElement root) {
			this.hotbarRoot = HOTBAR_ELEMENT.Get(root);
			this.BindHotbar(this.hotbarRoot);
		}

		private void BindHotbar(VisualElement hotbarRoot) {
			this.hotbarDisplays = new UXMLHotbarSlotDisplay[PlayerInventory.HOTBAR_SIZE];

			foreach (int slotIndex in this.playerInventory.GetHotbar()) {
				IItemSlot slot = this.playerInventory.GetSlot(slotIndex);
				VisualElement slotElement = hotbarRoot[slotIndex];

				UXMLHotbarSlotDisplay display = new(slot, this.itemRenderManager, true);
				this.AddWidget(display);
				display.OnBind(slotElement);
				this.hotbarDisplays[slotIndex] = display;
			}

			this.playerInventory.mainSlotChanged += this.OnMainSlotChanged;
			this.SetAsMainSlotVisual(this.playerInventory.GetMainSlot());
		}

		public void SetHotbarVisible(bool visible) {
			if (this.hotbarRoot != null) {
				this.hotbarRoot.visible = visible;
			}
		}

		private void OnMainSlotChanged(int oldIndex, int newIndex) {
			this.UnsetMainSlotVisual(oldIndex);
			this.SetAsMainSlotVisual(newIndex);
		}

		private void SetAsMainSlotVisual(int slot) {
			this.hotbarDisplays[slot].SetAsMainSlot();
		}

		private void UnsetMainSlotVisual(int slot) {
			this.hotbarDisplays[slot].UnsetMainSlot();
		}

		public override void OnDispose(IScreenHandle handle) {
			base.OnDispose(handle);

			for (int i = 0; i < this.hotbarDisplays.Length; i++) {
				this.hotbarDisplays[i].Dispose();
			}
		}
	}
}
