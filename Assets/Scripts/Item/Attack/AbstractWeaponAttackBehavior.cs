using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AbstractWeaponAttackBehavior : ScriptableObject {
	public abstract void Setup(AttackHandler attackHandler);
	
	public virtual void PostAttack(AttackHandler attackHandler) => Destroy(attackHandler);

	public abstract Dictionary<string, Action<AttackHandler>> AnimationEventsSupplier { get; }

	public abstract void OnHit(AttackHandler attackHandler);

	public void Destroy(AttackHandler attackHandler) => GameObject.Destroy(attackHandler.transform.parent.gameObject);

	public AttackHandlerEvents GenerateEvents() => new(Setup, AnimationEventsSupplier, PostAttack, OnHit);
}
