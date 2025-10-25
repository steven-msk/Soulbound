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
		public event Action<Collider2D>? onHitFrame;
		public event Action<Collider2D>? onHitboxEnter;
		public event Action<Collider2D>? onHitboxExit;
		public event AnimationEvent? onSpawnHitbox;
		public event AnimationEvent? onDespawnHitbox;

		public void OnAttackStart() {
			onAttackStart?.Invoke();
		}

		public void SpawnHitbox() {
			onSpawnHitbox?.Invoke();
		}

		public void DespawnHitbox() {
			onDespawnHitbox?.Invoke();
		}

		public void OnHitFrame(Collider2D other) {
			onHitFrame?.Invoke(other);
		}

		public void OnHitboxEnter(Collider2D other) {
			onHitboxEnter?.Invoke(other);
		}

		public void OnHitboxExit(Collider2D other) {
			onHitboxExit?.Invoke(other);
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
