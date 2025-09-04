using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable enable

public class ArmorSlot : EquipmentSlot, IItemSlot {
	[SerializeField] private ArmorType armorType;
	public ArmorType AcceptedType => armorType;

	[SerializeField] private GameObject overlay;
	public override int index { get; set; }
	public override bool showTooltip { get; set; } = true;

	public override IItemContainer2D container => this.GetComponentInParent<InventoryController>();

	bool IItemSlot.Handshake(ItemDisplay? grabbedItem, SlotInteractionMode interactionMode) {
		if (interactionMode == SlotInteractionMode.Drag) {
			return false;
		}
		if (grabbedItem?.DisplayedItem is ArmorItem armor && armor.armorType == this.AcceptedType) {
			if (this.HasItem) {
				this.CastDisplayed()!.OnUnequipped();
			}
			armor.OnEquip(this);
			overlay.SetActive(false);
			return true;
		}
		if (grabbedItem == null && this.HasItem) {
			this.CastDisplayed()?.OnUnequipped();
			overlay.SetActive(true);
			return true;
		}
		return false;
	}

	private ArmorItem? CastDisplayed() => this.ItemStack?.item as ArmorItem;
}