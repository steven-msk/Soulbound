using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "Items/WeaponItem")]
public class WeaponItem : Item, IAttackPerformer {
	[SerializeField] private List<SerializableStat> baseStats;

	public void PerformAttack(PlayerController player) {
		Debug.Log("attack");
	}

	protected override AbstractTooltip GetDefaultTooltip() {
		return CompoundTooltip.Of(Tooltip.Title(itemName), Tooltip.Stats(baseStats), Tooltip.Info(infoTextTooltip), Tooltip.Lore(loreTextTooltip));
	}
}