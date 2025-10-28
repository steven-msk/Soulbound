using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Unity.VisualScripting.Member;

#nullable enable

namespace SoulboundBackend.Client.Combat {
	public sealed class AttackAnimationHandler {
		private readonly Action animationTrigger;
		private readonly AttackEventDispatcher eventDispatcher;
		private AttackHandler attackHandler = null!;
		private AnimationEvent? onAnimationStart;
		private AnimationEvent? onAnimationEnd;
		private AnimationEvent spawnHitbox = null!;
		private AnimationEvent despawnHitbox = null!;

		public AttackAnimationHandler(Action animationTrigger, AttackEventDispatcher eventDispatcher) {
			this.animationTrigger = animationTrigger;
			this.eventDispatcher = eventDispatcher;
		}

		public void BindEvents(
				AnimationEvent? onAnimationStart,
				AnimationEvent? onAnimationEnd,
				AnimationEvent spawnHitbox, 
				AnimationEvent despawnHitbox
			) {
			this.onAnimationStart = onAnimationStart;
			this.onAnimationEnd = onAnimationEnd;
			this.spawnHitbox = spawnHitbox;
			this.despawnHitbox = despawnHitbox;

			eventDispatcher.onAttackAnimationStart += onAnimationStart;
			eventDispatcher.onAttackAnimationEnd += OnAttackAnimationEnd;
			eventDispatcher.onSpawnHitbox += spawnHitbox;
			eventDispatcher.onDespawnHitbox += despawnHitbox;
		}

		public void UnbindEvents() {
			eventDispatcher.onAttackAnimationStart -= onAnimationStart;
			eventDispatcher.onAttackAnimationEnd -= OnAttackAnimationEnd;
			eventDispatcher.onSpawnHitbox -= spawnHitbox;
			eventDispatcher.onDespawnHitbox -= despawnHitbox;
		}

		public void StartAnimation(AttackHandler handler) {
			this.attackHandler = handler;
			animationTrigger.Invoke();
			eventDispatcher.OnAttackAnimationStart();
		}

		private void OnAttackAnimationEnd() {
			onAnimationEnd?.Invoke();
			attackHandler?.OnAttackAnimationEnd();
		}
	}
}
