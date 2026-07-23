using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.Item.Container;
using SoulboundEngine.Client.UI;
using SoulboundEngine.Core.Registry;
using System;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.Render.Item {
	public class UXMLItemSlotDisplay : UXMLWidget, IUXMLItemSlotDisplay {
		private static readonly Identifier ITEM_DISPLAY_ELEMENT = Identifier.Of("soulbound:slot/item_display");
		private static readonly Identifier STACK_COUNT_ELEMENT = Identifier.Of("soulbound:slot/stack_count");
		protected readonly IItemSlot slot;
		protected readonly ItemRenderManager itemRenderManager;
		protected readonly ItemRenderHandle renderHandle;
		protected ItemStack stack;
		public event Action<PointerDownEvent>? onPointerDown;
		public event Action<PointerEnterEvent>? onPointerEnter;
		public event Action<PointerLeaveEvent>? onPointerLeave;
		public event Action<PointerUpEvent>? onPointerUp;
		private bool isHovering;
		private bool interactable;
		private bool isTooltipVisible;
		private bool showTooltip;

		public UXMLItemSlotDisplay(IItemSlot slot, ItemRenderManager itemRenderManager, bool interactable, bool showTooltip = true) {
			this.interactable = interactable;
			this.showTooltip = showTooltip;
			this.slot = slot;
			this.itemRenderManager = itemRenderManager;
			this.renderHandle = new ItemRenderHandle(this);
		}

		public override void OnBind(VisualElement root) {
			this.root = root;
			this.stack = this.slot.GetStack();
			this.slot.stackChanged += this.StackChanged;
			if (this.interactable) this.RegisterPointerCallbacks();

			this.Render();
		}

		protected void StackChanged(ItemStack oldStack, ItemStack newStack) => this.SetStack(newStack);

		protected void SetStack(ItemStack stack) {
			this.stack = stack;
			this.Render();
			this.UpdateTooltip();
		}

		protected void Render() {
			if (this.stack.IsEmpty()) {
				this.itemRenderManager.Destroy(this.renderHandle, this.RenderContext);
				return;
			}
			this.itemRenderManager.Render(this.renderHandle, this.stack, this.RenderContext);
		}

		public override void Dispose() {
			this.itemRenderManager.Destroy(this.renderHandle, this.RenderContext);
			this.slot.stackChanged -= this.StackChanged;
			if (this.interactable) this.UnregisterPointerCallbacks();
			onPointerDown = null;
			onPointerEnter = null;
			onPointerLeave = null;
			onPointerUp = null;
			if (this.isTooltipVisible) this.Screen.ClearTooltip();
		}

		public bool IsInteractable() => this.interactable;

		public void SetInteractable(bool interactable) {
			bool previouslyInteractable = this.interactable;
			this.interactable = interactable;
			if (interactable && !previouslyInteractable) {
				this.RegisterPointerCallbacks();
			} else if (!interactable) {
				this.UnregisterPointerCallbacks();
			}
		}

		public void ShowTooltip(bool showTooltip) {
			this.showTooltip = showTooltip;
		}

		private void RegisterPointerCallbacks() {
			this.root.RegisterCallback<PointerDownEvent>(this.OnPointerDown);
			this.root.RegisterCallback<PointerUpEvent>(this.OnPointerUp);
			this.root.RegisterCallback<PointerEnterEvent>(this.OnPointerEnter);
			this.root.RegisterCallback<PointerLeaveEvent>(this.OnPointerLeave);

		}

		private void UnregisterPointerCallbacks() {
			this.root.UnregisterCallback<PointerDownEvent>(this.OnPointerDown);
			this.root.UnregisterCallback<PointerUpEvent>(this.OnPointerUp);
			this.root.UnregisterCallback<PointerEnterEvent>(this.OnPointerEnter);
			this.root.UnregisterCallback<PointerLeaveEvent>(this.OnPointerLeave);

		}

		public virtual void SetAsMainSlot() {
		}

		public virtual void UnsetMainSlot() {
		}

		protected virtual ItemRenderContext RenderContext => new ItemRenderContext.UIToolkit(this.root, ITEM_DISPLAY_ELEMENT, STACK_COUNT_ELEMENT);

		private void OnPointerEnter(PointerEnterEvent evt) {
			onPointerEnter?.Invoke(evt);
			this.isHovering = true;
			this.UpdateTooltip();
		}

		private void OnPointerLeave(PointerLeaveEvent evt) {
			onPointerLeave?.Invoke(evt);
			this.isHovering = false;
			this.UpdateTooltip();
		}

		private void UpdateTooltip() {
			if (!this.showTooltip) return;
			if (this.isHovering && !this.stack.IsEmpty()) {
				this.Screen.SetTooltip(this.stack.item.name);
				this.isTooltipVisible = true;
			} else {
				this.Screen.ClearTooltip();
			}
		}

		private void OnPointerDown(PointerDownEvent evt) => onPointerDown?.Invoke(evt);

		private void OnPointerUp(PointerUpEvent evt) => onPointerUp?.Invoke(evt);

	}
}
