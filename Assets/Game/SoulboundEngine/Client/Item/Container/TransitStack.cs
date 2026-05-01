using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Core.Render.Sprite;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	public sealed class TransitStack {
		private readonly RectTransform parent;
		private Vector2 pointerPosition;
		private ItemStack? itemStack;
		private UIItemView? itemView;

		private readonly UIItemRenderer itemRenderer = new(new AtlasSpriteResolver());
		private readonly ItemModelResolver modelResolver = new();

		public TransitStack(RectTransform parent) {
			this.parent = parent;
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
			ItemRenderData renderData = itemStack.item.GetRenderData(itemStack);
			ItemRenderModel model = this.modelResolver.Resolve(renderData);
			this.itemView = this.itemView != null ? this.itemView : this.itemRenderer.CreateView(this.parent);

			this.itemRenderer.Render(this.itemView, model);
			this.itemView.SetParent(this.parent);
			this.itemView.SetPosition(this.pointerPosition);
		}

		public bool HasStack() => this.itemView != null;
		public ItemStack? GetStack() => this.itemStack;

		public void Destroy() {
			if (this.itemView == null) return;

			this.itemView.Destroy();
			if (this.itemStack != null) {
				this.itemStack.onQuantityChanged -= this.OnStackQuantityChanged;
			}
			this.itemView = null;
			this.itemStack = null;
		}

		public void SetPointerPosition(Vector2 position) {
			this.pointerPosition = position;
			if (this.itemView != null) {
				this.itemView.SetPosition(this.pointerPosition);
			}
		}
	}
}
