using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTag {
	Weapon,
	Consumable,
	Armor,
	CraftingMaterial,
	//....
}

public static class ItemTagBehavior {
	public static void ExecuteBehavior(this ItemTag tag) {
		Debug.Log($"{tag} behavior executed");
	}
}
