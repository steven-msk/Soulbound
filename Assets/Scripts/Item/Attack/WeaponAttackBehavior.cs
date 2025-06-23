using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class WeaponAttackBehavior : ScriptableObject {
	public abstract void PreAttack(AttackHandler attackHandler);
	
	public virtual void PostAttack(AttackHandler attackHandler, string attack) => Destroy(attackHandler);

	public abstract Dictionary<string, Action<AttackHandler>> AnimationEventsSupplier { get; }

	public abstract HashSet<ItemUseTrigger> RecognizedTriggers { get; }

	public abstract Func<ItemUseTrigger, AttackProcedure> AttackProcedureSupplier { get; }

	public abstract void OnHit(AttackHandler attackHandler);

	public void Destroy(AttackHandler attackHandler) => GameObject.Destroy(attackHandler.transform.parent.gameObject);

	public AttackHandlerEvents GenerateEvents() => new(PreAttack, AnimationEventsSupplier, PostAttack, OnHit);
}
