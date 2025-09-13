using SoulboundBackend.Common.Item.Attack;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem.Attack {
	public class AttackHandlerEvents {
		private readonly Action<GameObject> preAttack;
		private readonly Action<GameObject, string>? postAttack;
		private readonly Action<GameObject /*, ILivingEntity */>? onHit;                // PLANNED: onHit events should take ILivingEntity hitTarget as a parameter
		private readonly Dictionary<string, Action<GameObject>> animationEvents = new();

		public AttackHandlerEvents(Action<GameObject> preAttack, Dictionary<string, Action<GameObject>> animationEvents,
				Action<GameObject, string>? postAttack, Action<GameObject>? onHit) {
			this.preAttack = preAttack;
			this.animationEvents = animationEvents;
			this.postAttack = postAttack;
			this.onHit = onHit;
		}

		public void PreAttack(GameObject attackHandlerParent) => preAttack.Invoke(attackHandlerParent);

		public void PostAttack(GameObject attackHandlerParent, string attack) => postAttack?.Invoke(attackHandlerParent, attack);

		public void OnHit(GameObject attackHandlerParent) => onHit?.Invoke(attackHandlerParent);

		public void InvokeAnimationEvent(string name, GameObject attackHandlerParent) {
			if (animationEvents.TryGetValue(name, out Action<GameObject> @event)) {
				@event.Invoke(attackHandlerParent);
			} else {
				throw new AttackAnimationEventNotFoundException(name, attackHandlerParent.GetComponentInChildren<AttackHandler>());
			}
		}
		public class AttackAnimationEventNotFoundException : NullReferenceException {
			public AttackAnimationEventNotFoundException(string eventName, AttackHandler attackHandler)
				: base($"Attack animation '{eventName}' not found in '{attackHandler.name}'") { }
		}
	}
}
