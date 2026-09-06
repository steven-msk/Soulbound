namespace SoulboundEngine.UnityClient.UI.Screen {
	using SoulboundEngine.Item;
	using SoulboundEngine.Item.Container;
	using SoulboundEngine.UnityClient.Assets;
	using SoulboundEngine.UnityClient.Render.Item;
	using SoulboundEngine.UnityClient.UI.UXMLBindings;
	using SoulboundEngine.World.Player;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UIElements;

#nullable enable

	public sealed class TransitStackHandler : UXMLItemSlotDisplay, IInventory {
		private static readonly UXMLBinding<VisualElement> TRANSIT_STACK_ELEMENT = new("soulbound:transit_stack/transit_stack");
		private static readonly UXMLBinding<VisualElement> ITEM_DISPLAY_ELEMENT = new("soulbound:transit_stack/item_display");
		private static readonly UXMLBinding<Label> STACK_COUNT_ELEMENT = new("soulbound:transit_stack/stack_count");
		private static readonly UXMLBinding<ProgressBar> DURABILITY_BAR_ELEMENT = new("soulbound:transit_stack/durability_bar");
		private Vector2 pointerPosition;

		private TransitStackHandler(ItemRenderManager itemRenderManager, VisualElement root) 
			: base(itemRenderManager, false, false) {
			this.OnBind(root, new ItemSlot(this, 0));
		}

		public static TransitStackHandler Create(VisualElement screenRoot, ItemRenderManager itemRenderManager) {
			return new TransitStackHandler(itemRenderManager, CreateVisualElement(screenRoot));
		}

		protected override VisualElement GetDisplayElement(VisualElement root) => ITEM_DISPLAY_ELEMENT.Get(root);

		protected override Label GetStackCountElement(VisualElement root) => STACK_COUNT_ELEMENT.Get(root);

		protected override ProgressBar GetDurabilityBar(VisualElement root) => DURABILITY_BAR_ELEMENT.Get(root);

		protected override void Render(ItemStack stack) {
			base.Render(stack);
			this.UpdateViewPosition();
		}

		public void Destroy() {
			this.Dispose();
			this.SetStackDontRender(ItemStack.EMPTY);
		}

		public void SetPointerPosition(Vector2 position) {
			Vector2 panelPosition = this.root.panel != null
				? RuntimePanelUtils.ScreenToPanel(this.root.panel, position)
				: position;

			this.pointerPosition = this.root.parent != null
				? this.root.parent.WorldToLocal(panelPosition)
				: panelPosition;

			this.UpdateViewPosition();
		}

		private void UpdateViewPosition() {
			Vector2 size = this.root.worldBound.size;
			Vector2 pos = this.pointerPosition - size / 2f;
			this.root.style.left = pos.x;
			this.root.style.bottom = pos.y;
		}

		IItemSlot IInventory.GetSlot(int index) => this.slot!;

		IEnumerable<int> IInventory.GetSlots() => new[] { 0 };

		int IInventory.GetSize() => 1;

		bool IInventory.CanPlayerUse(PlayerEntity player) => true;

		private static VisualElement CreateVisualElement(VisualElement screenRoot) {
			// TODO: rework UI asset resolution
			VisualTreeAsset asset = AssetManager.Resolve<VisualTreeAsset>(new AssetKey("TransitStack"));
			asset.CloneTree(screenRoot);
			return TRANSIT_STACK_ELEMENT.Get(screenRoot);

			//VisualElement root = new() {
			//	name = "TransitStack",
			//	pickingMode = PickingMode.Ignore,
			//};
			//root.style.position = Position.Absolute;
			//screenRoot.Add(root);

			//VisualElement itemDisplay = new() {
			//	name = "ItemDisplay",
			//	pickingMode = PickingMode.Ignore
			//};
			//itemDisplay.style.flexShrink = 0f;
			//itemDisplay.style.width = itemDisplay.style.height = ITEM_DISPLAY_SIZE;
			//root.Add(itemDisplay);

			//Label stackCount = new() {
			//	name = "StackCount",
			//	pickingMode = PickingMode.Ignore
			//};
			//stackCount.style.right = 2f;
			//stackCount.style.width = ITEM_DISPLAY_SIZE;

			//return root;
		}
	}
}
