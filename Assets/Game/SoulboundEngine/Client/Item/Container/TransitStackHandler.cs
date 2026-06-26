using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Core.Assets;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.Item.Container {
	public sealed class TransitStackHandler {
		const float ITEM_DISPLAY_SIZE = 48f;
		private readonly VisualElement root;
		private Vector2 pointerPosition;
		private ItemStack itemStack;
		private IItemView? itemView;
		private readonly ItemRenderManager itemRenderManager;
		private readonly ItemRenderHandle renderHandle;

		public TransitStackHandler(ItemRenderManager itemRenderManager, VisualElement root) {
			this.itemRenderManager = itemRenderManager;
			this.root = root;
			this.renderHandle = new ItemRenderHandle(this);
		}

		public static TransitStackHandler Create(VisualElement screenRoot, ItemRenderManager itemRenderManager) {
			return new TransitStackHandler(itemRenderManager, CreateVisualElement(screenRoot));
		}

		private static VisualElement CreateVisualElement(VisualElement screenRoot) {
			// TODO: this is so fucking bad
			VisualTreeAsset asset = AssetManager.Resolve<VisualTreeAsset>(new AssetKey("TransitStack"));
			asset.CloneTree(screenRoot);
			return screenRoot.Q<VisualElement>("TransitStack");

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
			this.itemView = this.itemRenderManager.Render(this.renderHandle, itemStack, this.RenderContext);
			this.UpdateViewPosition();
		}

		public bool HasStack() => this.itemView != null;
		public ItemStack GetStack() => this.itemStack;

		public void Destroy() {
			if (this.itemView == null) return;

			this.itemRenderManager.Destroy(this.renderHandle, this.RenderContext);
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

		private ItemRenderContext RenderContext => new ItemRenderContext.UIToolkit { root = this.root };
	}
}
