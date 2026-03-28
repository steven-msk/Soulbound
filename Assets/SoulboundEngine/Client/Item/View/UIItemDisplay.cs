using SoulboundEngine.Client.ItemSystem.Render;
using System;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.View {
	public class UIItemDisplay : IItemDisplay {
		public event Action? onDestroyed;
		private UIItemView view;
		private ItemStack? itemStack;
		private bool destroyed = false;

		// placeholder
		private readonly UIItemRenderer itemRenderer = new();
		private readonly ItemModelResolver modelResolver = new();

		public UIItemDisplay(RectTransform parent, ItemStack itemStack) {
			view = itemRenderer.CreateView(parent);
			SetStack(itemStack);
		}

		public void SetStack(ItemStack? itemStack) {
			if (IsDestroyed()) {
				Logger.LogError("Cannot set UI item display stack: object already destroyed");
				return;
			}
			ItemStack? oldStack = this.itemStack;
			this.itemStack = itemStack;

			if (oldStack != null) {
				oldStack.onQuantityChanged -= OnStackQuantityChanged;
			}

			if (itemStack == null) Destroy();
			else {
				itemStack.onQuantityChanged += OnStackQuantityChanged;
				Render(itemStack);
			}
		}

		private void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) {
				Destroy();
				return;
			}
			if (itemStack != null) Render(itemStack);
		}

		private void Render(ItemStack itemStack) {
			ItemRenderData renderData = itemStack.item.GetRenderData(itemStack);
			ItemRenderModel model = modelResolver.Resolve(renderData);
			itemRenderer.Render(view, model);
		}

		public void SetPosition(Vector2 position) {

			// placeholder
			RectTransform rect = view.GetComponent<RectTransform>();
			rect.transform.position = position;
		}
		public void SetParent(RectTransform parent) {

			// placeholder
			RectTransform rect = view.GetComponent<RectTransform>();
			rect.SetParent(parent, false);
		}

		public void Destroy() {
			onDestroyed?.Invoke();
			destroyed = true;
			itemStack = null;
			view.Destroy();
			view = null!;
		}

		public ItemStack? GetStack() => itemStack;

		public bool IsDestroyed() => destroyed;
	}
}
