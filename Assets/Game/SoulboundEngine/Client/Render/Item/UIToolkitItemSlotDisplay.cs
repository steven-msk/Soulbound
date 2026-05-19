using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.UI;
using System;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.Render.Item {
	public class UIToolkitItemSlotDisplay : UxmlWidget, IDisposable {
		protected readonly IItemSlot slot;
		protected readonly ItemRenderManager itemRenderManager;
		protected readonly ItemRenderHandle renderHandle;
		protected ItemStack? stack;

		public UIToolkitItemSlotDisplay(IItemSlot slot, ItemRenderManager itemRenderManager) {
			this.slot = slot;
			this.itemRenderManager = itemRenderManager;
			this.renderHandle = new ItemRenderHandle(this);
		}

		public override void OnBind(VisualElement root) {
			this.root = root;
			this.slot.stackChanged += this.StackChanged;
			this.SetStack(this.slot.GetStack());
		}

		protected void StackChanged(ItemStack? oldStack, ItemStack? newStack) => this.SetStack(newStack);

		protected void SetStack(ItemStack? stack) {
			if (this.stack != null) {
				this.stack.onQuantityChanged -= this.OnStackQuantityChanged;
			}

			if (stack != null) {
				stack.onQuantityChanged += this.OnStackQuantityChanged;
			}

			this.stack = stack;
			this.Render();
		}

		protected void OnStackQuantityChanged(int oldCount, int newCount) {
			if (newCount <= 0) {
				this.itemRenderManager.Destroy(this.renderHandle, this.RenderContext);
				return;
			}

			this.Render();
		}

		protected void Render() {
			if (this.stack == null || this.stack.item == Items.AIR) {
				this.itemRenderManager.Destroy(this.renderHandle, this.RenderContext);
				return;
			}

			this.itemRenderManager.Render(this.renderHandle, this.stack, this.RenderContext);
		}

		public void Dispose() {
			this.itemRenderManager.Destroy(this.renderHandle, this.RenderContext);

			this.slot.stackChanged -= this.StackChanged;

			if (this.stack != null) {
				this.stack.onQuantityChanged -= this.OnStackQuantityChanged;
			}

			this.stack = null;
		}

		public virtual void SetAsMainSlot() {
		}

		public virtual void UnsetMainSlot() {
		}

		protected ItemRenderContext RenderContext => new ItemRenderContext.UIToolkit { root = this.root };
	}
}
