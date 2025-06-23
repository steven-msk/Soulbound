using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AttackHandlerEvents {
	private readonly Action<AttackHandler> preAttack;
	private readonly Action<AttackHandler, string> postAttack;
	private readonly Action<AttackHandler /*, ILivingEntity */> onHit;				// PLANNED: onHit events should take ILivingEntity hitTarget as a parameter
	private readonly Dictionary<string, Action<AttackHandler>> animationEvents = new();

	public AttackHandlerEvents([AllowsNull] Action<AttackHandler> setup, Dictionary<string, Action<AttackHandler>> animationEvents, 
			[AllowsNull] Action<AttackHandler, string> postAttack, [AllowsNull] Action<AttackHandler> onHit) {
		this.preAttack = setup ?? (_ => { });
		this.animationEvents = animationEvents;
		this.postAttack = postAttack ?? DefaultPostAttack;
		this.onHit = onHit ?? (_ => { });
	}

	public void PreAttack(AttackHandler attackHandler) => preAttack.Invoke(attackHandler);

	public void PostAttack(AttackHandler attackHandler, string attack) => postAttack.Invoke(attackHandler, attack);

	public void OnHit(AttackHandler attackHandler) => onHit.Invoke(attackHandler);

	public void InvokeAnimationEvent(string name, AttackHandler attackHandler) {
		if (animationEvents.TryGetValue(name, out Action<AttackHandler> @event)) {
			@event.Invoke(attackHandler);
		} else {
			throw new AttackAnimationEventNotFoundException(name, attackHandler);
		}
	}

	private static void DefaultPostAttack(AttackHandler attackHandler, string attack) => GameObject.Destroy(attackHandler.transform.parent.gameObject);

	private class AttackAnimationEventNotFoundException : NullReferenceException {
		public AttackAnimationEventNotFoundException(string eventName, AttackHandler attackHandler)
			: base($"Attack animation '{eventName}' not found in '{attackHandler.name}'") { }
	}
}