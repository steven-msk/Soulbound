using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "Items/WeaponItem")]
public class WeaponItem : Item {
	[SerializeField]
	private Dictionary<IStatTypeImpl, object> baseStats = new() {
		[StatType<int>.PhysicalDamage] = 25,
		[StatType<int>.RitualDamage] = 20,
		[StatType<float>.AttackSpeed] = 1.2f,
		[StatType<float>.CritChance] = 0.05f,
		[StatType<float>.CritMultiplier] = 1.2f
	};

	protected override AbstractTooltip GetDefaultTooltip() {
 		return CompoundTooltip.Of(Tooltip.Title(itemName), Tooltip.Stats(baseStats), Tooltip.Lore(loreTextTooltip));
	}
}

[Serializable]
public struct Stat {
	public string name;
	public float value;

	public Stat(string name, float value) {
		this.name = name;
		this.value = value;
	}
}