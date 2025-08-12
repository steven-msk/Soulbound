using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// FIXME: incomplete weapon item fields and behavior

public abstract class WeaponAttackBehavior {
	public abstract void PreAttack(GameObject attackHandlerParent);
	
	public virtual void PostAttack(GameObject attackHandlerParent, string attack) => GameObject.Destroy(attackHandlerParent);

	public abstract Dictionary<string, Action<GameObject>> AnimationEventsSupplier { get; }

	public abstract HashSet<ItemUseTrigger> RecognizedTriggers { get; }

	public abstract Func<ItemUseTrigger, AttackProcedure> AttackProcedureSupplier { get; }

	public abstract void OnHit(GameObject attackHandlerParent);

	public AttackHandlerEvents GenerateEvents() => new(PreAttack, AnimationEventsSupplier, PostAttack, OnHit);
}
