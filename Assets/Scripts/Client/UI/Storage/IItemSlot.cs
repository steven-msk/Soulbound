using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public interface IItemSlot : IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISerializable<SerializedItemSlot> {
		public ItemDisplay ItemDisplay { get; }
		public IItemContainer? container { get; }
		public int index { get; set; }
		public Transform transform { get; }
		public bool showTooltip { get; set; }

		public ItemStack? ItemStack => ItemDisplay?.ItemStack;
		public Item? ContainedItem => ItemStack?.item;

		public bool HasItem => ItemDisplay != null;
		public bool IsEmpty => ItemDisplay == null;

		internal void AttachItemDisplay(ItemDisplay itemDisplay, bool suppressHook = false) {
			itemDisplay.OnRelease(transform);
			OnAttached(itemDisplay, suppressHook);
		}

		private void OnAttached(ItemDisplay itemDisplay, bool suppressHook = false) {
			if (suppressHook) {
				return;
			}
			itemDisplay.DisplayedItem?.GetSlotHook()?.onAttached?.Invoke(itemDisplay, this);
		}

		private void OnDetached(ItemDisplay itemDisplay, bool suppressHook = false) {
			if (suppressHook) {
				return;
			}
			itemDisplay.DisplayedItem?.GetSlotHook()?.onDetached?.Invoke(itemDisplay, this);
		}

		internal void NotifyDeserializedHook() => OnAttached(ItemDisplay);

		internal void DetachItemDisplay(Transform newParent, bool suppressHook = false) {
			if (ItemDisplay == null) {
				return;
			}
			ItemDisplay detached = this.ItemDisplay;
			ItemDisplay.OnGrab(newParent, true);
			this.OnDetached(detached, suppressHook);
		}

		SerializedItemSlot ISerializable<SerializedItemSlot>.Serialize() => new(index, ItemStack);

		public void TrySetStack(int quantity, Item fallback) {
			CreateDisplayIfEmpty(new ItemStack(fallback, quantity), out var display);
			this.ItemStack!.SetQuantity(quantity);
		}

		public int TryAddStack(int add, Item fallback) {
			if (!CreateDisplayIfEmpty(new ItemStack(fallback, 0), out var display)) {
				return this.ItemStack!.Increment(add);
			}
			this.ItemStack!.Increment(add);
			return add;
		}

		public ItemDisplay CreateDisplay(ItemStack itemStack, bool suppressHook = false) {
			ItemDisplay display = ItemDisplay.Create(itemStack, () => transform);
			this.OnAttached(display, suppressHook);
			container?.OnItemDisplayAdded(display, this);
			return display;
		}

		public bool CreateDisplayIfEmpty(ItemStack itemStack, out ItemDisplay? display) {
			if (this.ItemStack == null) {
				display = CreateDisplay(itemStack);
				return true;
			}
			display = null;
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
				ItemDisplay?.ShowTooltip(eventData.position, container.transform);
			}
		}

		new public virtual void OnPointerExit(PointerEventData eventData) {
			container.OnPointerExit(this, eventData);
			ItemDisplay?.DestroyTooltip();
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
			return slot.CreateDisplay(serialized.itemStack, suppressHook: true);
		}
	}
}