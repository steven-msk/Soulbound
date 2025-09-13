using SoulboundBackend.Client.ItemSystem.Attack;
using SoulboundBackend.Common.Item.Attack;
using UnityEngine;

namespace SoulboundBackend.Client.ItemSystem {
	public abstract class WeaponItem : StatItem, IAttackPerformer {
		public override bool applyInstantStatsOnHoverOrSelect => true;
		public abstract GameObject attackPrefab { get; protected set; }
		public abstract WeaponAttackBehavior attackBehavior { get; protected set; }

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
		// Most weapon attacks will consist in slashing, slicing, or swinging.


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
	}
}