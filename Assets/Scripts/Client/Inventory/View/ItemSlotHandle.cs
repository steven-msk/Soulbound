using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	[RequireComponent(typeof(RectTransform))]
	public class ItemSlotHandle : MonoBehaviour, IItemSlotHandle, ITooltipTrigger, IItemSlotEvents {
		private IItemSlot slot = null!;
		private ITooltip tooltip = null!;
		private ITooltipRenderer tooltipRenderer = null!;
		private ITooltipHandle? tooltipHandle;
		private RectTransform rect = null!;
		public event Action<int, PointerEventData>? pointerDown;
		public event Action<int, PointerEventData>? pointerUp;
		public event Action<int, PointerEventData>? pointerEnter;
		public event Action<int, PointerEventData>? pointerExit;
		private UIItemDisplay? activeDisplay;
		private ItemStack? stack;

		public void Init(IItemSlot slot) {
			this.slot = slot;
			rect = GetComponent<RectTransform>();
			slot.setStack += SetStack;
			SetStack(slot.GetStack());
		}

		private void SetStack(ItemStack? stack) {
			if (this.stack != null) this.stack.onQuantityChanged -= OnStackQuantityChanged;
			activeDisplay?.Destroy();
			OnStackQuantityChanged(this.stack?.quantity ?? 0, stack?.quantity ?? 0);
			this.stack = stack;

			if (stack != null) {
				activeDisplay = new UIItemDisplay(rect, stack);
				DestroyTooltip();
				stack.onQuantityChanged += OnStackQuantityChanged;

				// prototypical
				SetTooltip(new ItemTooltip(stack.item));
			} else {
				SetTooltip(null!);
				activeDisplay = null;
			}
		}

		void ITooltipTrigger.Init(ITooltipRenderer tooltipRenderer) => this.tooltipRenderer = tooltipRenderer;
		public void SetTooltip(ITooltip tooltip) => this.tooltip = tooltip;
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
			pointerEnter?.Invoke(slot.GetIndex(), eventData);
			if (tooltip != null) {
				tooltipHandle = tooltipRenderer.RenderTooltip(tooltip);
			}
		}
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
			pointerExit?.Invoke(slot.GetIndex(), eventData);
			DestroyTooltip();
		}
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
			pointerDown?.Invoke(slot.GetIndex(), eventData);
		}
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
			pointerUp?.Invoke(slot.GetIndex(), eventData);
		}

		private void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) {
				DestroyTooltip();
				tooltip = null!;

				if (stack != null) {
					stack.onQuantityChanged -= OnStackQuantityChanged;
				}
				stack = null;
			}
		}

		public void SetVisible(bool visible) {
			gameObject.SetActive(visible);
			if (!visible) DestroyTooltip();
		}

		private void DestroyTooltip() {
			tooltipHandle?.Destroy();
			tooltipHandle = null;
		}

		public void ToggleVisibility() => SetVisible(!gameObject.activeSelf);
	}
}
