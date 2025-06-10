using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "Items/WeaponItem")]
public class WeaponItem : Item {
	[SerializeField] private List<SerializableStat> baseStats;

	protected override AbstractTooltip GetDefaultTooltip() {
		return CompoundTooltip.Of(Tooltip.Title(itemName), Tooltip.Stats(baseStats), Tooltip.Info(infoTextTooltip), Tooltip.Lore(loreTextTooltip));
	}
}