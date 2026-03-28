using SoulboundEngine.Client.ItemSystem.Container.View;
using SoulboundEngine.Client.ItemSystem.Render;
using System;
using UnityEngine;



#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	public sealed class TransitStack {
		private readonly RectTransform parent;
		private readonly IItemContainerScreenScope screenScope;
		private Vector2 pointerPosition;
		private ItemStack? itemStack;
		private UIItemView? itemView;

		// placeholder
		private readonly UIItemRenderer itemRenderer = new();
		private readonly ItemModelResolver modelResolver = new();

		public TransitStack(RectTransform parent) {
			this.parent = parent;
		}

		public void SetStack(ItemStack itemStack) {
			if (itemStack == null) {
				UnityEngine.Debug.LogException(new ArgumentException("TransitStack cannot be set to null. Call Release() instead"));
				return;
			}

			this.itemStack = itemStack;
			itemStack.onQuantityChanged += OnStackQuantityChanged;

			Render(itemStack);
		}

		private void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) Destroy();
			else if (itemStack != null && itemView != null) Render(itemStack);
		}

		private void Render(ItemStack itemStack) {
			ItemRenderData renderData = itemStack.item.GetRenderData(itemStack);
			ItemRenderModel model = modelResolver.Resolve(renderData);
			itemView = itemView != null ? itemView : itemRenderer.CreateView(parent);

			itemRenderer.Render(itemView, model);
			itemView.SetParent(parent);
			itemView.SetPosition(pointerPosition);
		}

		public bool HasStack() => itemView != null;
		public ItemStack? GetStack() => itemStack;

		public void Destroy() {
			if (itemView == null) return;

			itemView.Destroy();
			itemView = null;
			itemStack = null;
		}

		public void SetPointerPosition(Vector2 position) {
			this.pointerPosition = position;
			if (itemView != null) {
				itemView.SetPosition(pointerPosition);
			}
		}
	}
}
