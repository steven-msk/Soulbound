using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "Items/WeaponItem")]
public class WeaponItem : Item, IAttackPerformer, IStatProvider {
	[SerializeField] private List<SerializableStat> baseStats;
	public List<SerializableStat> Stats => baseStats;

	public bool ApplyStatsAutomatically => true;

	[Header("Attack Constraints")]
	[SerializeField] private float windupTime;
	public float WindupTime => windupTime;
	[SerializeField] private float cooldown;
	public float Cooldown => cooldown;
	[SerializeField] private GameObject attackPrefab;

	// FEATUREIMPL: weapon attacks
	// Each armor and weapon item can be inscriptioned with +4 soul slots. This means that the
	// maximum amount of souls a player can have is 2 base + 4 * 5 = 22 souls max.
	// This can only be achieved in late post-inscriptioning due to the high rarity of the 4-soul-slotted
	// inscriptions, but the gameplay becomes increasingly chaotic because of the extremely high
	// number of rituals and/or affixes: 22 max souls * 2 or 3 (in extremely rare cases) = 44 or 66
	// rituals and/or affixes total. Each ritual has high impact on combat, while the affixes impact
	// the results of the combat. You can instantly notice that, with high-tier souls with powerful
	// rituals, the gameplay becomes extremely chaotic. Because of this, weapon attacks will be
	// simple; they should leave the role of the real combat (including detailed effects) to rituals
	// e.g. a quick slash, horizontal slice, stab, double slash, spinning attack (AoE), parry
	// As you can see, most weapon attacks will consist in slashing, slicing, or swinging.


	public void PerformAttack(PlayerController player) {
		GameObject attackObject = GameObject.Instantiate(attackPrefab); 
		attackObject.GetComponentInChildren<AttackHandler>().Init(player, this);
		attackObject.transform.position = player.transform.position;
	}

	protected override AbstractTooltip GetDefaultTooltip() {
		return CompoundTooltip.Of(Tooltip.Title(itemName), Tooltip.Stats(baseStats), Tooltip.Info(infoTextTooltip), Tooltip.Lore(loreTextTooltip));
	}
}