using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public interface IItemSlot : ISerializable<SerializedItemSlot> {
		[Obsolete] public ItemDisplay itemDisplay { get; }
		public IItemContainer container { get; }
		[Obsolete] public int index { get; set; }
		[Obsolete] public Transform transform { get; }
		[Obsolete] public bool showTooltip { get; set; }

		public ItemStack? stack => itemDisplay?.stack;
		public Item? item => stack?.item;

		[Obsolete] public bool hasItem => GetStack() != null;
		[Obsolete] public bool IsEmpty => GetStack() == null;

		event Action<ItemStack?> setStack;
		ItemStack? GetStack();
		void SetStack(ItemStack? stack);

		public bool HasStack() => GetStack()?.quantity > 0;

		[Obsolete] public void AttachItemDisplay(ItemDisplay itemDisplay) {
			itemDisplay.OnRelease(transform);
			item!.OnAttachedInSlot(this);
		}

		[Obsolete] public void DetachItemDisplay(Transform newParent) {
			if (itemDisplay == null) {
				return;
			}
			ItemDisplay detached = this.itemDisplay;
			itemDisplay.OnGrab(newParent, true);
			detached.item!.OnDetachedFromSlot(this);
		}

		SerializedItemSlot ISerializable<SerializedItemSlot>.Serialize() => new(index, stack);


		public int TryAddStack(int add, Item fallback) {
			if (!CreateDisplayIfEmpty(new ItemStack(fallback, 0), out var display)) {
				return this.stack!.Increment(add);
			}
			this.stack!.Increment(add);
			return add;
		}

		[Obsolete] public ItemDisplay CreateDisplay(ItemStack itemStack) {
			ItemDisplay display = ItemDisplay.Create(itemStack, () => transform);
			itemStack.item.OnAttachedInSlot(this);
			container.OnItemDisplayAdded(display, this);
			return display;
		}

		internal void InternalDeserialize(SerializedItemSlot slot) {
			ItemDisplay display = ItemDisplay.Create(slot.itemStack, () => transform);
			container.OnItemDisplayAdded(display, this);
		}

		[Obsolete] public bool CreateDisplayIfEmpty(ItemStack itemStack, out ItemDisplay? display) {
			if (this.stack == null) {
				display = CreateDisplay(itemStack);
				return true;
			}
			display = itemDisplay;
			return false;
		}

		/// <summary>
		/// Validates whether this slot agrees to interact with the given item upon the given interaction mode
		/// </summary>
		[Obsolete]
		virtual bool Handshake(ItemDisplay? grabbedItem, SlotInteractionMode interactionMode) {
			return interactionMode == SlotInteractionMode.Click ? !(grabbedItem == null && this.IsEmpty) : true;
		}
	}

	public static class ItemSlotDeserializer {
		[Obsolete]
		public static ItemDisplay Deserialize(this IItemSlot slot, SerializedItemSlot serialized) {
			slot.InternalDeserialize(serialized);
			return slot.itemDisplay;
		}
	}
}
