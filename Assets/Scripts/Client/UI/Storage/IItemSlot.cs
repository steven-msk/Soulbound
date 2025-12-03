using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public interface IItemSlot : IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISerializable<SerializedItemSlot> {
		public ItemDisplay itemDisplay { get; }
		public IItemContainer container { get; }
		public int index { get; set; }
		public Transform transform { get; }
		public bool showTooltip { get; set; }

		public ItemStack? stack => itemDisplay?.stack;
		public Item? item => stack?.item;

		public bool HasItem => itemDisplay != null;
		public bool IsEmpty => itemDisplay == null;

		public void AttachItemDisplay(ItemDisplay itemDisplay) {
			itemDisplay.OnRelease(transform);
			item!.OnAttachedInSlot(this);
		}

		public void DetachItemDisplay(Transform newParent) {
			if (itemDisplay == null) {
				return;
			}
			ItemDisplay detached = this.itemDisplay;
			itemDisplay.OnGrab(newParent, true);
			detached.item!.OnDetachedFromSlot(this);
		}

		SerializedItemSlot ISerializable<SerializedItemSlot>.Serialize() => new(index, stack);

		public void TrySetStack(int quantity, Item fallback) {
			CreateDisplayIfEmpty(new ItemStack(fallback, quantity), out var display);
			this.stack!.SetQuantity(quantity);
		}

		public int TryAddStack(int add, Item fallback) {
			if (!CreateDisplayIfEmpty(new ItemStack(fallback, 0), out var display)) {
				return this.stack!.Increment(add);
			}
			this.stack!.Increment(add);
			return add;
		}

		public ItemDisplay CreateDisplay(ItemStack itemStack) {
			ItemDisplay display = ItemDisplay.Create(itemStack, () => transform);
			itemStack.item.OnAttachedInSlot(this);
			container.OnItemDisplayAdded(display, this);
			return display;
		}

		internal void InternalDeserialize(SerializedItemSlot slot) {
			ItemDisplay display = ItemDisplay.Create(slot.itemStack, () => transform);
			container.OnItemDisplayAdded(display, this);
		}

		public bool CreateDisplayIfEmpty(ItemStack itemStack, out ItemDisplay? display) {
			if (this.stack == null) {
				display = CreateDisplay(itemStack);
				return true;
			}
			display = itemDisplay;
			return false;
		}

		new public virtual void OnPointerDown(PointerEventData eventData) {
			container.OnPointerDown(this, eventData);
		}
		new public virtual void OnPointerUp(PointerEventData eventData) {
			container.OnPointerUp(this, eventData);
		}
		new public virtual void OnPointerEnter(PointerEventData eventData) {
			container.OnPointerEnter(this, eventData);
			if (showTooltip) {
				itemDisplay?.ShowTooltip(eventData.position, container.transform);
			}
		}

		new public virtual void OnPointerExit(PointerEventData eventData) {
			container.OnPointerExit(this, eventData);
			itemDisplay?.DestroyTooltip();
		}

		/// <summary>
		/// Validates whether this slot agrees to interact with the given item upon the given interaction mode
		/// </summary>
		virtual bool Handshake(ItemDisplay? grabbedItem, SlotInteractionMode interactionMode) {
			return interactionMode == SlotInteractionMode.Click ? !(grabbedItem == null && this.IsEmpty) : true;
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData) => this.OnPointerDown(eventData);
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData) => this.OnPointerUp(eventData);
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => this.OnPointerEnter(eventData);
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => this.OnPointerExit(eventData);
	}

	public static class ItemSlotDeserializer {
		public static ItemDisplay Deserialize(this IItemSlot slot, SerializedItemSlot serialized) {
			slot.InternalDeserialize(serialized);
			return slot.itemDisplay;
		}
	}
}