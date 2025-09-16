using SoulboundBackend.Client.ItemSystem;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public class InventorySlot : MonoBehaviour, IItemSlot {
		public IItemContainer container => gameObject.GetComponentInParent<InventoryController>(true);
		public ItemDisplay? ItemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
		public int index { get; set; }
		public bool HasItem => ItemDisplay != null;
		public bool IsEmpty => ItemDisplay == null;

		public ItemStack? ItemStack => ItemDisplay?.ItemStack;
		public bool showTooltip { get; set; } = true;

		public GameObject GameObject => gameObject;

		public void Deserialize(SerializedItemSlot serialized) {
			ItemDisplay.Create(serialized.itemStack, this);
		}

		public void OnInventoryPopup(bool opened) {
			if (!opened && this.HasItem) {
				ItemDisplay!.DestroyTooltip();
			}
		}
	}
}