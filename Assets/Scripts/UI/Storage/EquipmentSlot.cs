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

	public void OnPointerDown(PointerEventData eventData) => GameManager.GetPlayerInstance().Inventory.OnEquipmentSlotClicked(this);
}
