using SoulboundEngine.World.Player;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Item.Container {
	public sealed class TransitStackHandler : UXMLItemSlotDisplay, IInventory {
		private static readonly Identifier TRANSIT_STACK_ELEMENT = Identifier.Of("soulbound:transit_stack/transit_stack");
		private static readonly Identifier ITEM_DISPLAY_ELEMENT = Identifier.Of("soulbound:transit_stack/item_display");
		private static readonly Identifier STACK_COUNT_ELEMENT = Identifier.Of("soulbound:transit_stack/stack_count");
		private static readonly Identifier DURABILITY_BAR_ELEMENT = Identifier.Of("soulbound:transit_stack/durability_bar");
		private Vector2 pointerPosition;

		private TransitStackHandler(ItemRenderManager itemRenderManager, VisualElement root) 
			: base(itemRenderManager, false, false) {
			this.OnBind(root, new ItemSlot(this, 0));
		}

		public static TransitStackHandler Create(VisualElement screenRoot, ItemRenderManager itemRenderManager) {
			return new TransitStackHandler(itemRenderManager, CreateVisualElement(screenRoot));
		}

		protected override Identifier GetItemDisplayId() => ITEM_DISPLAY_ELEMENT;
		protected override Identifier GetStackCountId() => STACK_COUNT_ELEMENT;
		protected override Identifier GetDurabilityBarId() => DURABILITY_BAR_ELEMENT;

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
			this.view?.SetPosition(pos);
		}

		IItemSlot IInventory.GetSlot(int index) => this.slot!;

		IEnumerable<int> IInventory.GetSlots() => new[] { 0 };

		int IInventory.GetSize() => 1;

		bool IInventory.CanPlayerUse(PlayerEntity player) => true;

		private static VisualElement CreateVisualElement(VisualElement screenRoot) {
			// TODO: rework UI asset resolution
			VisualTreeAsset asset = AssetManager.Resolve<VisualTreeAsset>(new AssetKey("TransitStack"));
			asset.CloneTree(screenRoot);
			return screenRoot.Get<VisualElement>(TRANSIT_STACK_ELEMENT);

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
