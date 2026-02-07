using SoulboundBackend.Client.ItemSystem;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.UI.Storage {
	public class InventorySlotHandle : MonoBehaviour, IItemSlot, IUIElementHandle {
		public IItemContainer container => gameObject.GetComponentInParent<InventoryController>(true);
		[Obsolete] public ItemDisplay? itemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
		public int index { get; set; }
		public bool HasItem => itemDisplay != null;
		public bool IsEmpty => itemDisplay == null;

		public ItemStack? stack => itemDisplay?.stack;
		public Item? item => stack?.item;
		[Obsolete] public bool showTooltip { get; set; } = true;

		private ItemStack? _stack;

		public void Init(ItemStack? stack) {
			_stack = stack;
		}

		public void Deserialize(SerializedItemSlot serialized) {
			ItemSlotDeserializer.Deserialize(this, serialized);
		}

		public void OnInventoryPopup(bool opened) {
			if (!opened && this.HasItem) {
				itemDisplay!.DestroyTooltip();
			}
		}

		ItemStack? IItemSlot.GetStack() => _stack;
		int IItemSlot.GetIndex() => index;

		public void SetVisible(bool visible) => gameObject.SetActive(visible);

		public void ToggleVisibility() {
			gameObject.SetActive(!gameObject.activeSelf);
		}
	}
}
