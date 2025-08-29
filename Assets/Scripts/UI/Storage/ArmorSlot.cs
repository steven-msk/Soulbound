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
	public override int index { get; set; }

	public override IItemContainer2D container => throw new NotImplementedException();

	//[InputAction("ItemDrag", Priority = 10, BlocksContexts = new[] { "ItemUse" })]
	//public override void OnClick(ItemDisplay grabbedItem, InventoryController inventory) {
	//	if ((grabbedItem?.ItemStack.item is ArmorItem armorItem && this.AcceptedType == armorItem.armorType) || (grabbedItem == null && this.HasItem)) {
	//		base.OnClick(grabbedItem, inventory);
	//		InvocationHelper.IfElse(this.ItemDisplay != null, HideOverlay, ShowOverlay);
	//	}
	//}

	public void HideOverlay() => overlay.SetActive(false);

	public void ShowOverlay() => overlay.SetActive(true);
}