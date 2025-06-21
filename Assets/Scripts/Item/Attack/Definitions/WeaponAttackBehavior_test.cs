using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon/Attack Behaviors/WeaponAttackBevahior_test")]
public class WeaponAttackBehavior_test : AbstractWeaponAttackBehavior {
	public override Dictionary<string, Action<AttackHandler>> AnimationEventsSupplier => new() {
		["AnimEvent"] = _ => Debug.Log("event")
	};

	public override void Setup(AttackHandler attackHandler) { 
		attackHandler.transform.position = GameManager.GetPlayerInstance().transform.position;
	}

	public override void PostAttack(AttackHandler attackHandler) {
		base.PostAttack(attackHandler);
		Debug.Log("destroy");
	}

	public override void OnHit(AttackHandler attackHandler) {
		Debug.Log("OnHit");
	}
}
