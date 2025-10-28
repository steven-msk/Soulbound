using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.Combat {
	public class AttackHandler {
		private readonly AttackSource source;
		private readonly AttackEventDispatcher eventDispatcher;
		private readonly IHitRecognizer hitRecognizer;
		public bool isHandlingAttack { get; private set; }
		private bool isHitboxActive = false;
		private Hitbox hitbox;

		public AttackHandler(AttackSource source, AttackEventDispatcher eventDispatcher, IHitRecognizer hitRecognizer) {
			this.source = source;
			this.eventDispatcher = eventDispatcher;
			this.hitbox = source.hitboxSupplier.Invoke();
			this.hitRecognizer = hitRecognizer;

			eventDispatcher.onAttackStart += StartAttack;
			eventDispatcher.onAttackEnd += EndAttack;
			eventDispatcher.onAttackAnimationStart += source.onAttackAnimationStart;
			eventDispatcher.onAttackAnimationEnd += source.onAttackAnimationEnd;
			eventDispatcher.onSpawnHitbox += SpawnHitbox;
			eventDispatcher.onDespawnHitbox += DespawnHitbox;
			eventDispatcher.onHitboxEnter += source.onHitboxEnter;
			eventDispatcher.onHitboxExit += source.onHitboxExit;
			eventDispatcher.onHitFrame += OnHitFrame;
		}

		public void StartAttack() {
			isHandlingAttack = true;
			source.onAttackStart?.Invoke();
		}

		public void EndAttack() {
			eventDispatcher.onAttackStart -= StartAttack;
			eventDispatcher.onAttackEnd -= EndAttack;
			eventDispatcher.onAttackAnimationStart -= source.onAttackAnimationStart;
			eventDispatcher.onAttackAnimationEnd -= source.onAttackAnimationEnd;
			eventDispatcher.onSpawnHitbox -= SpawnHitbox;
			eventDispatcher.onDespawnHitbox -= DespawnHitbox;
			eventDispatcher.onHitboxEnter -= source.onHitboxEnter;
			eventDispatcher.onHitboxExit -= source.onHitboxExit;
			eventDispatcher.onHitFrame -= OnHitFrame;

			InvocationHelper.If(isHitboxActive, DespawnHitbox);
			source.onAttackEnd?.Invoke();
		}

		private void SpawnHitbox() {
			isHitboxActive = true;
			hitbox.Activate(eventDispatcher);
			source.onHitboxSpawned?.Invoke(hitbox);
		}

		private void DespawnHitbox() {
			isHitboxActive = false;
			hitbox.Deactivate();
			source.onHitboxDespawned?.Invoke();
		}

		private void OnHitFrame(Collider2D other) {
			if (hitRecognizer.ShouldRegisterHit(other)) {
				source.onHitRegistered?.Invoke(other);
			}
			hitRecognizer.OnHitFrame(other);
			source.onHitFrame(other);
		}
	}
}
