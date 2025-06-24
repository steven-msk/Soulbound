using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon/Attack Behaviors/WeaponAttackBevahior_test")]
public class WeaponAttackBehavior_test : WeaponAttackBehavior {

	static readonly Dictionary<ItemUseTrigger, AttackProcedure> attacks = new() {
		[ItemUseTrigger.LeftHold] = AttackProcedure.Triggered("attack", 0.15f),
		[ItemUseTrigger.RightHold] = AttackProcedure.Triggered("attack1", 0.5f)
	};

	public override HashSet<ItemUseTrigger> RecognizedTriggers => attacks.Keys.ToHashSet();

	public override Dictionary<string, Action<GameObject>> AnimationEventsSupplier => new() {
		["AnimEvent"] = _ => Debug.Log("event")
	};

	public override void PreAttack(GameObject attackHandlerParent) {
		attackHandlerParent.transform.position = GameManager.GetPlayerInstance().transform.position;
	}

	public override Func<ItemUseTrigger, AttackProcedure> AttackProcedureSupplier => (trigger) => attacks.GetValueOrDefault(trigger, null);

	public override void PostAttack(GameObject attackHandlerParent, string attack) {
		base.PostAttack(attackHandlerParent, attack);
		Debug.Log(attack);
	}

	public override void OnHit(GameObject attackHandlerParent) {
		Debug.Log("OnHit");
	}
}
