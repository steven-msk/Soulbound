using SoulboundBackend.Common;
using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Client.Combat {
	public class AttackHandler {
		private static readonly Logger logger = Logger.CreateInstance();
		private readonly AttackSource source;
		private readonly AttackEventDispatcher eventDispatcher;
		private readonly IHitRecognizer hitRecognizer;
		private readonly bool shouldEndWhenAnimationEnds;
		public bool isHandlingAttack { get; private set; }
		private bool isHitboxActive = false;
		private Hitbox hitbox;
		private AttackAnimationHandler animationHandler;

		public AttackHandler(
				AttackSource source,
				AttackEventDispatcher eventDispatcher, 
				IHitRecognizer hitRecognizer,
				bool shouldEndWhenAnimationEnds
			) {
			this.source = source;
			this.eventDispatcher = eventDispatcher;
			this.hitbox = source.hitboxSupplier.Invoke();
			this.hitRecognizer = hitRecognizer;
			this.shouldEndWhenAnimationEnds = shouldEndWhenAnimationEnds;
		}

		public void StartAttack(AttackAnimationHandler animationHandler) {
			this.animationHandler = animationHandler;
			isHandlingAttack = true;

			animationHandler.BindEvents(source.onAttackAnimationStart, source.onAttackAnimationEnd, SpawnHitbox, DespawnHitbox);
			eventDispatcher.onAttackStart += source.onAttackStart;
			eventDispatcher.onAttackEnd += source.onAttackEnd;
			eventDispatcher.onHitFrame += OnHitFrame;

			eventDispatcher.OnAttackStart();
			animationHandler.StartAnimation(this);
		}

		public void EndAttack() {
			eventDispatcher.OnAttackEnd();
			InvocationHelper.If(isHitboxActive, DespawnHitbox);
			isHandlingAttack = false;

			animationHandler.UnbindEvents();
			eventDispatcher.onAttackStart -= source.onAttackStart;
			eventDispatcher.onAttackEnd -= source.onAttackEnd;
			eventDispatcher.onHitFrame -= OnHitFrame;
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
			if (other.TryGetComponent<Hurtbox>(out var hurtbox)) {
				if (hitRecognizer.ShouldRegisterHit(hurtbox)) {
					hurtbox.NotifyHit(source);
					source.onHitRegistered?.Invoke(other);
				}
				hitRecognizer.OnHitFrame(hurtbox);
				source.onHitFrame(other);
			} else {
				logger.LogError(null, "Could not find Hurtbox on hitbox collider: {}", other.gameObject);
			}
		}

		public void OnAttackAnimationEnd() {
			InvocationHelper.If(shouldEndWhenAnimationEnds, EndAttack);
		}
	}
}
