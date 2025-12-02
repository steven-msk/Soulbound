using SoulboundBackend.Client.ItemSystem;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public class InventorySlot : MonoBehaviour, IItemSlot {
		public IItemContainer container => gameObject.GetComponentInParent<InventoryController>(true);
		public ItemDisplay? itemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
		public int index { get; set; }
		public bool HasItem => itemDisplay != null;
		public bool IsEmpty => itemDisplay == null;

		public ItemStack? stack => itemDisplay?.stack;
		public Item? item => stack?.item;
		public bool showTooltip { get; set; } = true;

		public void Deserialize(SerializedItemSlot serialized) {
			ItemSlotDeserializer.Deserialize(this, serialized);
		}

		public void OnInventoryPopup(bool opened) {
			if (!opened && this.HasItem) {
				itemDisplay!.DestroyTooltip();
			}
		}
	}
}