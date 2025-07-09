using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IItemSlot {
	public ItemDisplay ItemDisplay => gameObject.GetComponentInChildren<ItemDisplay>();
	public bool HasItem => ItemDisplay != null;
	public bool IsEmpty => ItemDisplay == null;
	public ItemStack ItemStack => ItemDisplay?.ItemStack;

	public GameObject GameObject => gameObject;

	public virtual void OnPointerDown(PointerEventData eventData) { 
		InputHandler.RequestAction(new("ItemDrag", 10, () => GameManager.GetPlayerInstance().Inventory.OnEquipmentSlotClicked(this)));
		InputHandler.BlockContextUntil("ItemUse", () => GameManager.GetPlayerInstance().InputHandler.LeftHold);
	}
}
