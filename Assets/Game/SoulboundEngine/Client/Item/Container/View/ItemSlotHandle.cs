using SoulboundEngine.Client.Render.Item;
using SoulboundEngine.Client.UI.Tooltips;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container.View {
	[RequireComponent(typeof(RectTransform))]
	[Obsolete]
	public class ItemSlotHandle : MonoBehaviour, IItemSlotHandle, ITooltipTrigger {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		private IItemSlot slot;
		private ITooltip tooltip;
		private ITooltipRenderer tooltipRenderer;
		private IItemSlotEventListener eventListener;
		private ITooltipHandle? tooltipHandle;
		private RectTransform rect;
		private IItemView? itemView;
		private ItemStack? stack;
		private ItemRenderManager itemRenderManager;
		private ItemRenderHandle renderHandle;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		public void Init(ItemRenderManager itemRenderManager, IItemSlot slot, IItemSlotEventListener eventListener) {
			this.itemRenderManager = itemRenderManager;
			this.slot = slot;
			this.eventListener = eventListener;
			this.rect = this.GetComponent<RectTransform>();
			slot.setStack += this.SetStack;
			this.SetStack(slot.GetStack());
			this.renderHandle = new ItemRenderHandle(this);
		}

		private void SetStack(ItemStack? stack) {
			if (stack?.item == Items.AIR) return;

			if (this.stack != null) this.stack.onQuantityChanged -= this.OnStackQuantityChanged;
			this.OnStackQuantityChanged(this.stack?.quantity ?? 0, stack?.quantity ?? 0);
			this.stack = stack;

			if (stack != null) {
				this.DestroyTooltip();
				stack.onQuantityChanged += this.OnStackQuantityChanged;

				// prototypical
				this.SetTooltip(new ItemTooltip(stack.item));
			} else {
				this.SetTooltip(null!);
			}
			this.Render(stack);
		}

		private void Render(ItemStack? itemStack) {
			if (itemStack == null && this.itemView != null) {
				this.itemView.Destroy();
			} else if (itemStack != null && itemStack.item != Items.AIR) {
				this.itemView = this.itemRenderManager.Render(this.renderHandle, this.stack, new ItemRenderContext.GUI { parent = this.rect });
			}
		}

		void ITooltipTrigger.Init(ITooltipRenderer tooltipRenderer) => this.tooltipRenderer = tooltipRenderer;
		public void SetTooltip(ITooltip tooltip) => this.tooltip = tooltip;
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
			this.eventListener.OnPointerEnter(this.slot.GetIndex(), eventData);
			if (this.tooltip != null) {
				this.tooltipHandle = this.tooltipRenderer.RenderTooltip(this.tooltip);
			}
		}
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
			this.eventListener.OnPointerExit(this.slot.GetIndex(), eventData);
			this.DestroyTooltip();
		}
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
			this.eventListener.OnPointerDown(this.slot.GetIndex(), eventData);
		}
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
			this.eventListener.OnPointerUp(this.slot.GetIndex(), eventData);
		}

		private void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) {
				this.DestroyTooltip();
				this.tooltip = null!;

				if (this.stack != null) {
					this.stack.onQuantityChanged -= this.OnStackQuantityChanged;
				}
				this.stack = null;
				this.itemView?.Destroy();
			} else if (this.stack != null) this.Render(this.stack);
		}

		public void SetVisible(bool visible) {
			this.gameObject.SetActive(visible);
			if (!visible) this.DestroyTooltip();
		}

		private void DestroyTooltip() {
			this.tooltipHandle?.Destroy();
			this.tooltipHandle = null;
		}

		public void ToggleVisibility() => this.SetVisible(!this.gameObject.activeSelf);
	}
}
