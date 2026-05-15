using SoulboundEngine.Client.Render.Item;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	public sealed class TransitStack {
		private readonly VisualElement root;
		private Vector2 pointerPosition;
		private ItemStack? itemStack;
		private IItemView? itemView;
		private readonly ItemRenderManager itemRenderManager;
		private readonly ItemRenderHandle renderHandle;

		public TransitStack(ItemRenderManager itemRenderManager, VisualElement slot) {
			this.itemRenderManager = itemRenderManager;
			this.root = slot;
			this.renderHandle = new ItemRenderHandle(this);
		}

		public void SetStack(ItemStack itemStack) {
			if (itemStack == null) {
				UnityEngine.Debug.LogException(new ArgumentException("TransitStack cannot be set to null. Call Release() instead"));
				return;
			}

			if (this.itemStack != null) {
				this.itemStack.onQuantityChanged -= this.OnStackQuantityChanged;
			}

			this.itemStack = itemStack;
			itemStack.onQuantityChanged += this.OnStackQuantityChanged;

			this.Render(itemStack);
		}

		private void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) this.Destroy();
			else if (this.itemStack != null && this.itemView != null) this.Render(this.itemStack);
		}

		private void Render(ItemStack itemStack) {
			this.itemView = this.itemRenderManager.Render(this.renderHandle, itemStack, this.RenderContext);
			this.UpdateViewPosition();
		}

		public bool HasStack() => this.itemView != null;
		public ItemStack? GetStack() => this.itemStack;

		public void Destroy() {
			if (this.itemView == null) return;

			this.itemRenderManager.Destroy(this.renderHandle, this.RenderContext);
			if (this.itemStack != null) {
				this.itemStack.onQuantityChanged -= this.OnStackQuantityChanged;
			}
			this.itemView = null;
			this.itemStack = null;
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
