using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.Combat {
	public delegate void AnimationEvent();

	public class AttackEventDispatcher : MonoBehaviour {
		public event Action? onAttackStart;
		public event Action? onAttackEnd;
		public event AnimationEvent? onAttackAnimationStart;
		public event AnimationEvent? onAttackAnimationEnd;
		public event Action<Hitbox, Collider2D>? onHitFrame;
		public event Action<Hitbox, Collider2D>? onHitboxEnter;
		public event Action<Hitbox, Collider2D>? onHitboxExit;
		public event AnimationEvent enrollBehaviorContext = null!;
		public event AnimationEvent endBehaviorContext = null!;

		public void OnAttackStart() {
			onAttackStart?.Invoke();
		}

		public void EnrollBehaviorContext() {
			enrollBehaviorContext.Invoke();
		}

		public void EndBehaviorContext() {
			endBehaviorContext.Invoke();
		}

		public void OnHitFrame(Hitbox hitbox, Collider2D other) {
			onHitFrame?.Invoke(hitbox, other);
		}

		public void OnHitboxEnter(Hitbox hitbox, Collider2D other) {
			onHitboxEnter?.Invoke(hitbox, other);
		}

		public void OnHitboxExit(Hitbox hitbox, Collider2D other) {
			onHitboxExit?.Invoke(hitbox, other);
		}

		public void OnAttackAnimationStart() {
			onAttackAnimationStart?.Invoke();
		}

		public void OnAttackAnimationEnd() {
			onAttackAnimationEnd?.Invoke();
		}

		public void OnAttackEnd() {
			onAttackEnd?.Invoke();
		}
	}
}
