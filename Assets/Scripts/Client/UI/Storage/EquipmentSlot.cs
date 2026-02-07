using SoulboundBackend.Client.ItemSystem;
using System;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Storage {
	public abstract class EquipmentSlot : MonoBehaviour, IItemSlot {
		public abstract IItemContainer container { get; }
		public ItemDisplay itemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
		public abstract int index { get; set; }
		public bool HasItem => itemDisplay != null;
		public bool IsEmpty => itemDisplay == null;
		public ItemStack stack => itemDisplay?.stack;
		public abstract bool showTooltip { get; set; }

		public abstract void Deserialize(SerializedItemSlot serialized);

		ItemStack IItemSlot.GetStack() => stack;
		int IItemSlot.GetIndex() => index;

		//[InputAction("ItemDrag", Priority = 10, BlocksContexts = new[] { "ItemUse" })]
		//public virtual void OnClick(ItemDisplay grabbedItem, InventoryController inventory) {
		//	if ((grabbedItem?.ItemStack.item is not IEquipable && grabbedItem != null) || (grabbedItem == null && this.IsEmpty)) {
		//		return;
		//	}
		//	bool justEquipped = false;
		//	if (grabbedItem?.ItemStack.item is IEquipable equipable && !this.HasItem) {
		//		equipable.OnEquip(this);
		//		justEquipped = true;

		//	} else if (this.HasItem) {
		//		((IEquipable)this.ItemStack.item).OnUnequipped();
		//	}

		//	this.TransferGrabbed(grabbedItem, inventory);
		//	if (this.HasItem && !justEquipped) {
		//		((IEquipable)this.ItemStack.item).OnEquip(this);
		//	}
		//}

		//public virtual void OnPointerDown(PointerEventData eventData) => this.RequestClickAction();
	}
}
