using System;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.Combat {
	public record AttackAnimatorChannel(Animator animator, AttackEventDispatcher eventDispatcher) {
		public void Unwrap(ref Animator animator, ref AttackEventDispatcher eventDispatcher) {
			animator = this.animator;
			eventDispatcher = this.eventDispatcher;
		}

		public static AttackAnimatorChannel FromDelegates(
				Func<Animator> animator, 
				Func<AttackEventDispatcher> eventDispatcher
			) {
			return new AttackAnimatorChannel(
				animator(),
				eventDispatcher()
			);
		}
	}
}
