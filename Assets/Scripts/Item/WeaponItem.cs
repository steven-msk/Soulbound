using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "Items/WeaponItem")]
public class WeaponItem : Item, IAttackPerformer, IStatProvider {
	[SerializeField] private List<SerializableStat> baseStats;
	public List<SerializableStat> Stats => baseStats;

	public bool ApplyStatsAutomatically => true;

	public void PerformAttack(PlayerController player) {
		Debug.Log("attack");
	}

	protected override AbstractTooltip GetDefaultTooltip() {
		return CompoundTooltip.Of(Tooltip.Title(itemName), Tooltip.Stats(baseStats), Tooltip.Info(infoTextTooltip), Tooltip.Lore(loreTextTooltip));
	}
}