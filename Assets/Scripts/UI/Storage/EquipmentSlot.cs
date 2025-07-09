using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EquipmentSlot : MonoBehaviour, IItemSlot {
	public ItemDisplay ItemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
	public bool HasItem => ItemDisplay != null;
	public bool IsEmpty => ItemDisplay == null;
	public ItemStack ItemStack => ItemDisplay?.ItemStack;

	public GameObject GameObject => gameObject;

	[InputAction("ItemDrag", Priority = 10, BlocksContexts = new[] { "ItemUse" })]
	public virtual void OnClick(ItemDisplay grabbedItem, InventoryController inventory) {
		if (grabbedItem?.ItemStack.Item is not IEquipable && grabbedItem != null) {
			return;
		}
		bool justEquipped = false;
		if (grabbedItem?.ItemStack.Item is IEquipable equipable && !this.HasItem) {
			equipable.OnEquip(this);
			justEquipped = true;

		} else if (this.HasItem) {
			((IEquipable)this.ItemStack.Item).OnUnequipped();
		}

		this.TranserItems(grabbedItem, inventory);
		if (this.HasItem && !justEquipped) {
			((IEquipable)this.ItemStack.Item).OnEquip(this);
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData) { 
		InventoryController inventory = GameManager.GetPlayerInstance().Inventory;
		InputHandler.RequestAction(new("ItemDrag", 10, () => this.OnClick(inventory.GrabbedItem, inventory)));
		InputHandler.BlockContextUntil("ItemUse", () => GameManager.GetPlayerInstance().InputHandler.LeftHold);
	}
}
