using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using System;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.Render.Item {
	public class UIToolkitItemSlotHandle : IDisposable {
		private readonly VisualElement root;
		private readonly IItemSlot slot;
		private readonly ItemRenderManager itemRenderManager;
		private readonly ItemRenderHandle renderHandle;
		private ItemStack? stack;
		private IItemView? view;
		public event Action<PointerDownEvent>? onPointerDown;
		public event Action<PointerUpEvent>? onPointerUp;
		public event Action<PointerEnterEvent>? onPointerEnter;
		public event Action<PointerLeaveEvent>? onPointerLeave;

		public UIToolkitItemSlotHandle(VisualElement visualElement, IItemSlot slot, ItemRenderManager itemRenderManager) {
			this.root = visualElement;
			this.slot = slot;
			this.itemRenderManager = itemRenderManager;
			this.renderHandle = new ItemRenderHandle(this);

			slot.stackChanged += this.StackChanged;
			this.SetStack(slot.GetStack());

			visualElement.RegisterCallback<PointerDownEvent>(this.OnPointerDown);
			visualElement.RegisterCallback<PointerUpEvent>(this.OnPointerUp);
			visualElement.RegisterCallback<PointerEnterEvent>(this.OnPointerEnter);
			visualElement.RegisterCallback<PointerLeaveEvent>(this.OnPointerLeave);
		}

		private void StackChanged(ItemStack? oldStack, ItemStack? newStack) => this.SetStack(newStack);

		private void SetStack(ItemStack? stack) {
			if (this.stack != null) {
				this.stack.onQuantityChanged -= this.OnStackQuantityChanged;
			}

			if (stack != null) {
				stack.onQuantityChanged += this.OnStackQuantityChanged;
			}

			this.stack = stack;
			this.Render();
		}

		private void OnStackQuantityChanged(int oldCount, int newCount) {
			if (newCount <= 0) {
				this.itemRenderManager.Destroy(this.renderHandle);
				return;
			}

			this.Render();
		}

		private void Render() {
			if (this.stack == null || this.stack.item == Items.AIR) {
				this.itemRenderManager.Destroy(this.renderHandle);
				return;
			}

			this.view = this.itemRenderManager.Render(this.renderHandle, this.stack, new ItemRenderContext.UIToolkit { root = this.root });
		}

		public void Dispose() {
			this.itemRenderManager.Destroy(this.renderHandle);

			this.slot.stackChanged -= this.StackChanged;

			if (this.stack != null) {
				this.stack.onQuantityChanged -= this.OnStackQuantityChanged;
			}

			this.stack = null;

			this.root.UnregisterCallback<PointerDownEvent>(this.OnPointerDown);
			this.root.UnregisterCallback<PointerUpEvent>(this.OnPointerUp);
			this.root.UnregisterCallback<PointerEnterEvent>(this.OnPointerEnter);
			this.root.UnregisterCallback<PointerLeaveEvent>(this.OnPointerLeave);
		}

		private void OnPointerDown(PointerDownEvent evt) => onPointerDown?.Invoke(evt);
		private void OnPointerUp(PointerUpEvent evt) => onPointerUp?.Invoke(evt);
		private void OnPointerEnter(PointerEnterEvent evt) => onPointerEnter?.Invoke(evt);
		private void OnPointerLeave(PointerLeaveEvent evt) => onPointerLeave?.Invoke(evt);
	}
}
