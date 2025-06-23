using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon/Attack Behaviors/WeaponAttackBevahior_test")]
public class WeaponAttackBehavior_test : AbstractWeaponAttackBehavior {

	static readonly Dictionary<ItemUseTrigger, AttackProcedure> attacks = new() {
		[ItemUseTrigger.LeftHold] = AttackProcedure.Triggered("attack", 1f),
		[ItemUseTrigger.RightHold] = AttackProcedure.Triggered("attack1", 0.1f)
	};

	public override HashSet<ItemUseTrigger> RecognizedTriggers => attacks.Keys.ToHashSet();

	public override Dictionary<string, Action<AttackHandler>> AnimationEventsSupplier => new() {
		["AnimEvent"] = _ => Debug.Log("event")
	};

	public override void PreAttack(AttackHandler attackHandler) {
		attackHandler.transform.position = GameManager.GetPlayerInstance().transform.position;
	}

	public override Func<ItemUseTrigger, AttackProcedure> AttackProcedureSupplier => (trigger) => attacks.GetValueOrDefault(trigger, null);

	public override void PostAttack(AttackHandler attackHandler) {
		base.PostAttack(attackHandler);
		Debug.Log("destroy");
	}

	public override void OnHit(AttackHandler attackHandler) {
		Debug.Log("OnHit");
	}
}
