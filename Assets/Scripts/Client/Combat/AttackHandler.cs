using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.Combat {
	public class AttackHandler {
		private readonly AttackSource source;
		private readonly AttackEventDispatcher eventDispatcher;
		private bool isHitboxActive = false;

		public AttackHandler(AttackSource source, AttackEventDispatcher eventDispatcher) {
			this.source = source;
			this.eventDispatcher = eventDispatcher;

			eventDispatcher.onAttackStart += StartAttack;
			eventDispatcher.onAttackEnd += EndAttack;
			eventDispatcher.onAttackAnimationStart += source.onAttackAnimationStart;
			eventDispatcher.onAttackAnimationEnd += source.onAttackAnimationEnd;
			eventDispatcher.onSpawnHitbox += SpawnHitbox;
			eventDispatcher.onDespawnHitbox += DespawnHitbox;
			//eventDispatcher.onHitFrame +=
		}

		public void StartAttack() {

			source.onAttackStart?.Invoke();
		}

		public void EndAttack() {
			eventDispatcher.onAttackStart -= StartAttack;
			eventDispatcher.onAttackEnd -= EndAttack;
			eventDispatcher.onAttackAnimationStart -= source.onAttackAnimationStart;
			eventDispatcher.onAttackAnimationEnd -= source.onAttackAnimationEnd;
			eventDispatcher.onSpawnHitbox -= SpawnHitbox;
			eventDispatcher.onDespawnHitbox -= DespawnHitbox;
			InvocationHelper.If(isHitboxActive, DespawnHitbox);
			source.onAttackEnd?.Invoke();
		}

		private void SpawnHitbox() {
			isHitboxActive = true;
			source.onHitboxSpawned?.Invoke(default);
			//source.onHitboxSpawned?.Invoke(hitbox);
		}

		private void DespawnHitbox() {
			isHitboxActive = false;
			source.onHitboxDespawned?.Invoke();
		}
	}
}
