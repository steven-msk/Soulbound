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
		private AnimationEvent enrollBehaviorContext = null!;
		private AnimationEvent endBehaviorContext = null!;

		public AttackAnimationHandler(Action animationTrigger, AttackEventDispatcher eventDispatcher) {
			this.animationTrigger = animationTrigger;
			this.eventDispatcher = eventDispatcher;
		}

		public void BindEvents(
				AnimationEvent? onAnimationStart,
				AnimationEvent? onAnimationEnd,
				AnimationEvent enrollBehaviorContext, 
				AnimationEvent endBehaviorContext
			) {
			this.onAnimationStart = onAnimationStart;
			this.onAnimationEnd = onAnimationEnd;
			this.enrollBehaviorContext = enrollBehaviorContext;
			this.endBehaviorContext = endBehaviorContext;

			eventDispatcher.onAttackAnimationStart += onAnimationStart;
			eventDispatcher.onAttackAnimationEnd += onAnimationEnd;
			eventDispatcher.enrollBehaviorContext += this.enrollBehaviorContext;
			eventDispatcher.endBehaviorContext += this.endBehaviorContext;
		}

		public void UnbindEvents() {
			eventDispatcher.onAttackAnimationStart -= onAnimationStart;
			eventDispatcher.onAttackAnimationEnd -= onAnimationEnd;
			eventDispatcher.enrollBehaviorContext -= enrollBehaviorContext;
			eventDispatcher.endBehaviorContext -= endBehaviorContext;
		}

		public void StartAnimation(AttackHandler handler) {
			this.attackHandler = handler;
			animationTrigger.Invoke();
			eventDispatcher.OnAttackAnimationStart();
		}
	}
}
