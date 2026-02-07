using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Core;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public class InventorySlotHandle : MonoBehaviour, IItemSlot, IItemSlotHandle, ITooltipTrigger, IPointerEnterHandler, IPointerExitHandler {
		[Obsolete] private IItemSlot slot = null!;
		[Obsolete] private IItemContainer container = null!;
		private ITooltipDefinition tooltip = null!;
		private ITooltipRenderer tooltipRenderer = null!;
		private ITooltipHandle? tooltipHandle;
		[Obsolete] public ItemDisplay? itemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
		public int index { get; set; }
		public bool HasItem => itemDisplay != null;
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

		public void Init(IItemSlot slot, IItemContainer container) {
			this.slot = slot;
			this.container = container;

			slot.setStack += SetStack;
			SetStack(slot.GetStack());
		}

		private void SetStack(ItemStack? stack) {
			_stack = stack;
			if (activeDisplay != null) activeDisplay.Destroy();
			if (stack != null) {
				activeDisplay = ItemDisplay.Create(stack, () => transform);
				DestroyTooltip();
				// prototypical
				((ITooltipTrigger)this).SetTooltip(new ItemTooltip(stack.item));
			}
		}

		void ITooltipTrigger.Init(ITooltipRenderer tooltipRenderer) => this.tooltipRenderer = tooltipRenderer;
		void ITooltipTrigger.SetTooltip(ITooltipDefinition tooltip) => this.tooltip = tooltip;
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
			tooltipHandle = tooltipRenderer.RenderTooltip(tooltip);
		}
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => DestroyTooltip();

		[Obsolete]
		public void OnInventoryPopup(bool opened) {
			if (!opened && this.HasItem) {
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

		int IItemSlot.GetIndex() {
			throw new NotImplementedException();
		}

		void ISerializable<SerializedItemSlot>.Deserialize(SerializedItemSlot serialized) {
			throw new NotImplementedException();
		}
	}
}
