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
	[SerializeField] private List<Stat> baseStats;

	protected override AbstractTooltip GetDefaultTooltip() {
		Dictionary<string, object> stats = new(baseStats.Select(stat => new KeyValuePair<string, object>(stat.name, stat.value)));
 		return CompoundTooltip.Of(Tooltip.Title(itemName), Tooltip.Stats(stats), Tooltip.Lore(loreTextTooltip));
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