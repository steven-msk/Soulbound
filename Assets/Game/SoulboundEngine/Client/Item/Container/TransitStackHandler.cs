using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Registry;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public sealed class TransitStackHandler {
		private static readonly Identifier TRANSIT_STACK_ELEMENT = Identifier.Of("soulbound:transit_stack/transit_stack");
		private static readonly Identifier ITEM_DISPLAY_ELEMENT = Identifier.Of("soulbound:transit_stack/item_display");
		private static readonly Identifier STACK_COUNT_ELEMENT = Identifier.Of("soulbound:transit_stack/stack_count");
		const float ITEM_DISPLAY_SIZE = 48f;
		private readonly VisualElement root;
		private Vector2 pointerPosition;
		private ItemStack itemStack;
		private IItemView? itemView;
		private readonly ItemRenderManager itemRenderManager;
		private readonly ItemRenderHandle renderHandle;
		private readonly ItemRenderContext renderContext;

		private TransitStackHandler(ItemRenderManager itemRenderManager, VisualElement root) {
			this.itemRenderManager = itemRenderManager;
			this.root = root;
			this.renderHandle = new ItemRenderHandle(this);
			this.renderContext = new ItemRenderContext.UXML(this.root, ITEM_DISPLAY_ELEMENT, STACK_COUNT_ELEMENT);
		}

		public static TransitStackHandler Create(VisualElement screenRoot, ItemRenderManager itemRenderManager) {
			return new TransitStackHandler(itemRenderManager, CreateVisualElement(screenRoot));
		}

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

		public void SetStack(ItemStack itemStack) {
			if (itemStack.IsEmpty()) {
				this.Destroy();
				return;
			}

			this.itemStack = itemStack;
			this.Render(itemStack);
		}

		private void Render(ItemStack itemStack) {
			this.itemView = this.itemRenderManager.Render(this.renderHandle, itemStack, this.renderContext);
			this.UpdateViewPosition();
		}

		public bool HasStack() => this.itemView != null;
		public ItemStack GetStack() => this.itemStack;

		public void Destroy() {
			if (this.itemView == null) return;

			this.itemRenderManager.Destroy(this.renderHandle, this.renderContext);
			this.itemView = null;
			this.itemStack = ItemStack.EMPTY;
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
			this.itemView?.SetPosition(pos);
		}
	}
}
