using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AttackHandlerEvents {
	private readonly Action<AttackHandler> setup;
	private readonly Action<AttackHandler> postAttack;
	private readonly Dictionary<string, Action<AttackHandler>> animationEvents = new();

	public AttackHandlerEvents([AllowsNull] Action<AttackHandler> setup, Dictionary<string, Action<AttackHandler>> animationEvents, [AllowsNull] Action<AttackHandler> postAttack) {
		this.setup = setup ?? (_ => { });
		this.animationEvents = animationEvents;
		this.postAttack = postAttack ?? DefaultPostAttack;
	}

	public void Setup(AttackHandler attackHandler) => setup.Invoke(attackHandler);

	public void PostAttack(AttackHandler attackHandler) => postAttack.Invoke(attackHandler);

	public void InvokeAnimationEvent(string name, AttackHandler attackHandler) {
		if (animationEvents.TryGetValue(name, out Action<AttackHandler> @event)) {
			@event.Invoke(attackHandler);
		} else {
			throw new AttackAnimationEventNotFoundException(name, attackHandler);
		}
	}

	private static void DefaultPostAttack(AttackHandler attackHandler) => GameObject.Destroy(attackHandler.transform.parent.gameObject);

	private class AttackAnimationEventNotFoundException : NullReferenceException {
		public AttackAnimationEventNotFoundException(string eventName, AttackHandler attackHandler)
			: base($"Attack animation '{eventName}' not found in '{attackHandler.name}'") { }
	}
}