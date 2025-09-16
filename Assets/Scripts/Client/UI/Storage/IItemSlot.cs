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
		public IItemContainer container { get; }
		public int index { get; set; }
		public bool HasItem => ItemDisplay != null;
		public bool IsEmpty => ItemDisplay == null;
		public ItemStack? ItemStack => ItemDisplay?.ItemStack;
		public Item? ContainedItem => ItemStack?.item;
		public GameObject GameObject { get; }
		public bool showTooltip { get; set; }

		internal void AttachItemDisplay(ItemDisplay itemDisplay, bool suppressHook = false) {
			itemDisplay.transform.SetParent(GameObject.transform, true);
			itemDisplay.OnRelease();
			InvocationHelper.If(!suppressHook, () => OnAttached(itemDisplay));
		}

		private void OnAttached(ItemDisplay itemDisplay) {
			itemDisplay.DisplayedItem?.GetSlotHook()?.onAttached?.Invoke(itemDisplay, this);
		}

		internal void NotifyDeserializedHook() => OnAttached(ItemDisplay);

		internal void DetachItemDisplay(bool suppressHook = false) {
			if (!suppressHook) {
				ItemDisplay.DisplayedItem?.GetSlotHook()?.onDetached?.Invoke(ItemDisplay, this);
			}
			ItemDisplay.OnGrab();
			ItemDisplay?.transform.SetParent(GameManager.instance.Player.Inventory.transform, true);
		}

		SerializedItemSlot ISerializable<SerializedItemSlot>.Serialize() => new(index, ItemStack);

		public void TrySetStack(int quantity, Item fallback) {
			CreateDisplayIfEmpty(new ItemStack(fallback, quantity));
			this.ItemStack!.SetQuantity(quantity);
		}

		public int TryAddStack(int add, Item fallback) {
			if (!CreateDisplayIfEmpty(new ItemStack(fallback, 0))) {
				return this.ItemStack!.Increment(add);
			}
			this.ItemStack!.Increment(add);
			return add;
		}

		public void CreateDisplay(ItemStack itemStack, bool suppressHook = false) {
			this.AttachItemDisplay(ItemDisplay.Create(itemStack, () => GameObject.transform), suppressHook);
		}

		public bool CreateDisplayIfEmpty(ItemStack itemStack) {
			if (this.ItemStack == null) {
				CreateDisplay(itemStack);
				return true;
			}
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
			InvocationHelper.If(showTooltip, () => ItemDisplay?.ShowTooltip(eventData.position));
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
		public static void Deserialize(this IItemSlot slot, SerializedItemSlot serialized) {
			slot.CreateDisplay(serialized.itemStack, suppressHook: true);
		}
	}
}