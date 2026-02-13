using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public class ItemSlotHandle : MonoBehaviour, IItemSlotHandle, ITooltipTrigger, IItemSlotEvents {
		private IItemSlot slot = null!;
		private ITooltip tooltip = null!;
		private ITooltipRenderer tooltipRenderer = null!;
		private ITooltipHandle? tooltipHandle;
		public event Action<int, PointerEventData>? pointerDown;
		public event Action<int, PointerEventData>? pointerUp;
		public event Action<int, PointerEventData>? pointerEnter;
		public event Action<int, PointerEventData>? pointerExit;
		private ItemDisplay? activeDisplay;
		private ItemStack? _stack;

		public void Init(IItemSlot slot) {
			this.slot = slot;
			slot.setStack += SetStack;
			SetStack(slot.GetStack());
		}

		private void SetStack(ItemStack? stack) {
			if (_stack != null) _stack.onQuantityChanged -= OnStackQuantityChanged;
			if (activeDisplay != null) activeDisplay.Destroy();
			_stack = stack;

			if (stack != null) {
				activeDisplay = ItemDisplay.Create(stack, () => transform);
				DestroyTooltip();
				stack.onQuantityChanged += OnStackQuantityChanged;

				// prototypical
				SetTooltip(new ItemTooltip(stack.item));
			} else {
				SetTooltip(null!);
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
				_stack!.onQuantityChanged -= OnStackQuantityChanged;
				_stack = null;
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
