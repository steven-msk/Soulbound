using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public class InventorySlotHandle : MonoBehaviour, IItemSlot, IItemSlotHandle, ITooltipTrigger, IItemSlotEvents {
		private IItemSlot slot = null!;
		private ITooltipDefinition tooltip = null!;
		private ITooltipRenderer tooltipRenderer = null!;
		private ITooltipHandle? tooltipHandle;
		public event Action<int, PointerEventData>? pointerDown;
		public event Action<int, PointerEventData>? pointerUp;
		public event Action<int, PointerEventData>? pointerEnter;
		public event Action<int, PointerEventData>? pointerExit;
		[Obsolete] public ItemDisplay? itemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
		public int index { get; set; }
		public bool hasItem => itemDisplay != null;
		public bool IsEmpty => itemDisplay == null;

		public ItemStack? stack => itemDisplay?.stack;

		IItemContainer IItemSlot.container => throw new NotImplementedException();

		bool IItemSlot.showTooltip { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		private ItemDisplay? activeDisplay;
		private ItemStack? _stack;

		event Action<ItemStack?> IItemSlot.setStack {
			add {
				throw new NotImplementedException();
			}

			remove {
				throw new NotImplementedException();
			}
		}

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
		public void SetTooltip(ITooltipDefinition tooltip) => this.tooltip = tooltip;
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
			pointerEnter?.Invoke(slot.index, eventData);
			if (tooltip != null) {
				tooltipHandle = tooltipRenderer.RenderTooltip(tooltip);
			}
		}
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
			pointerExit?.Invoke(slot.index, eventData);
			DestroyTooltip();
		}
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
			pointerDown?.Invoke(slot.index, eventData);
		}
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
			pointerUp?.Invoke(slot.index, eventData);
		}

		private void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) {
				DestroyTooltip();
				tooltip = null!;
				_stack!.onQuantityChanged -= OnStackQuantityChanged;
				_stack = null;
			}
		}

		[Obsolete]
		public void OnInventoryPopup(bool opened) {
			if (!opened && this.hasItem) {
				itemDisplay!.DestroyTooltip();
			}
		}

		//IItemContainer IItemSlotHandle.GetContainer() => container;
		//IItemSlot IItemSlotHandle.GetSlot() => slot;

		public void SetVisible(bool visible) {
			gameObject.SetActive(visible);
			if (!visible) DestroyTooltip();
		}

		private void DestroyTooltip() {
			tooltipHandle?.Destroy();
			tooltipHandle = null;
		}

		public void ToggleVisibility() => SetVisible(!gameObject.activeSelf);

		ItemStack? IItemSlot.GetStack() {
			throw new NotImplementedException();
		}

		void IItemSlot.SetStack(ItemStack? stack) {
			throw new NotImplementedException();
		}

		void ISerializable<SerializedItemSlot>.Deserialize(SerializedItemSlot serialized) {
			throw new NotImplementedException();
		}
	}
}
