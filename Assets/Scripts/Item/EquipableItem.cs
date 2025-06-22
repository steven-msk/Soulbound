using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/EquipableItem")]
public class EquipableItem : Item, IEquipable {
	[SerializeField] private EquipableDelegate delegates;

	public void OnEquip(EquipmentSlot slot) => delegates.OnEquip(slot);

	public void OnUnequipped(Item item) => delegates.OnUnequip(item);
}
