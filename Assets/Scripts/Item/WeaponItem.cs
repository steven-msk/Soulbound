using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class WeaponItem : StatItemDefinition, IAttackPerformer {
	public override bool applyInstantStatsAutomatically => true;
	public abstract GameObject attackPrefab { get; protected set; }
	public abstract WeaponAttackBehavior attackBehavior { get; protected set; }

    public WeaponItem(string name, Sprite icon, Func<GameObject> worldPrefabSupplier, int maxStackSize, Func<Item, TooltipData?> tooltipSupplier,
			List<AbstractSerializableStat> instantStats, List<IBufferedStatImpl> bufferedStats, string interpolationSource,
			GameObject attackPrefab, WeaponAttackBehavior attackBehavior)
		: base(name, icon, worldPrefabSupplier, maxStackSize, tooltipSupplier, instantStats, bufferedStats, interpolationSource) {
		this.attackPrefab = attackPrefab;
		this.attackBehavior = attackBehavior;
    }

	// FEATUREIMPL (WIP): weapon attacks (NOT TESTED)
	// Each armor and weapon item can be inscriptioned with +4 soul slots. This means that the
	// maximum amount of souls a player can have is 2 base + 4 * 5 = 22 souls max.
	// This can only be achieved in late post-inscriptioning due to the high rarity of the 4-slotted
	// inscriptions, but the gameplay becomes increasingly chaotic because of the extremely high
	// number of rituals and/or affixes: 22 max souls * 2 or 3 (in extremely rare cases) = 44 or 66
	// rituals and/or affixes total. Each ritual has high impact on combat, while the affixes impact
	// the results of the combat. You can instantly notice that, with high-tier souls with powerful
	// rituals, the gameplay becomes extremely chaotic. Because of this, weapon attacks will be
	// simple; they should leave the role of the real combat (including detailed effects) to rituals
	// e.g. a quick slash, horizontal slice, stab, double slash, spinning attack (AoE), parry.
	// As you can see, most weapon attacks will consist in slashing, slicing, or swinging.


	public void PerformAttack(ItemUseTrigger trigger) {
		if (!attackBehavior.RecognizedTriggers.Contains(trigger)) {
			return;
		}
		GameObject attackObject = GameObject.Instantiate(attackPrefab);
		if (attackObject.GetComponentInChildren<AttackHandler>() == null) {
			throw new AttackHandlerNotFoundException(this, attackObject);
		}
		AttackHandler attackHandler = attackObject.GetComponentInChildren<AttackHandler>();
		attackHandler.Init(this, attackBehavior.GenerateEvents());
		attackHandler.HandleAttack(attackBehavior.AttackProcedureSupplier.Invoke(trigger));
	}

	public class AttackProcedureNotFoundException : NullReferenceException {
		public AttackProcedureNotFoundException(string weapon, ItemUseTrigger trigger)
			: base($"Weapon attack procedure not found: input: '{trigger}', weapon: '{weapon}'") { }
	}

	public class AttackHandlerNotFoundException : NullReferenceException {
		public AttackHandlerNotFoundException(WeaponItem weapon, GameObject attackObject) 
			: base($"AttackHandler not found in chilren of attack prefab asset. Item name: {weapon.name}, attack prefab: {attackObject.name}") { }
	}
}