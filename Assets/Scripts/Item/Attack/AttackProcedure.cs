using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AttackProcedure {
	private readonly Action<Animator> animatorTriggerAction;
	private readonly float cooldown;
	public float Cooldown => cooldown;

	public AttackProcedure(Action<Animator> animatorTriggerAction, float cooldown) {
		this.animatorTriggerAction = animatorTriggerAction;
		this.cooldown = cooldown;
	}

	public void InvokeAnimation(Animator animator) => animatorTriggerAction.Invoke(animator);

	public static AttackProcedure Triggered(string parameterName, float cooldown) => new(animator => animator.SetTrigger(parameterName), cooldown);
}
