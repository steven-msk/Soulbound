using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effects/EquipableDelegate_test")]
public class EquipableDelegate_test : EquipableDelegate {
	public override void OnEquip(EquipmentSlot slot) {
		Debug.Log("equip");
	}

	public override void OnUnequip(Item item) {
		Debug.Log("unequipped");
	}
}
