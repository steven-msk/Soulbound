using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArmorSlot : EquipmentSlot {
	[SerializeField] private ArmorType armorType;
	public ArmorType AcceptedType => armorType;

	[SerializeField] private GameObject overlay;
	public GameObject Overlay => overlay;

	public override void OnPointerDown(PointerEventData eventData) {
		InputHandler.RequestAction(new("ItemDrag", 10, () => GameManager.GetPlayerInstance().Inventory.OnArmorSlotClicked(this)));
		InputHandler.BlockContextUntil("ItemUse", () => GameManager.GetPlayerInstance().InputHandler.LeftHold);
	}

	public void HideOverlay() => overlay.SetActive(false);

	public void ShowOverlay() => overlay.SetActive(true);
}