using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EquipmentSlot : MonoBehaviour, IItemSlot {
	public abstract IItemContainer container { get; }
	public ItemDisplay ItemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
	public abstract int index { get; set; }
	public bool HasItem => ItemDisplay != null;
	public bool IsEmpty => ItemDisplay == null;
	public ItemStack ItemStack => ItemDisplay?.ItemStack; 
	public abstract bool showTooltip { get; set; }

	public GameObject GameObject => gameObject;

	public abstract void Deserialize(SerializedItemSlot serialized);

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
