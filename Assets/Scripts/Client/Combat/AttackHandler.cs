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
		public bool isHandlingAttack { get; private set; }
		private AttackAnimationHandler animationHandler;

		public AttackHandler(AttackSource source, AttackEventDispatcher eventDispatcher) {
			this.source = source;
			this.eventDispatcher = eventDispatcher;
		}

		public void StartAttack(AttackAnimationHandler animationHandler) {
			if (isHandlingAttack) {
				throw new InvalidOperationException("Attempted to start attack while an ongoing one is handled");
			}
			this.animationHandler = animationHandler;
			isHandlingAttack = true;

			animationHandler.BindEvents(
				source.behavior.OnAttackAnimationStart,
				source.behavior.OnAttackAnimationEnd, 
				EnrollBehaviorContext, 
				EndBehaviorContext
			);
			eventDispatcher.onAttackStart += source.behavior.OnAttackStart;
			eventDispatcher.onAttackEnd += source.behavior.OnAttackEnd;
			eventDispatcher.onHitFrame += OnHitFrame;

			eventDispatcher.OnAttackStart();
			animationHandler.StartAnimation(this);
		}

		public void EndAttack() {
			if (!isHandlingAttack) {
				throw new InvalidOperationException("Attempted to end attack when it hasnt even started yet");
			}
			eventDispatcher.OnAttackEnd();
			//InvocationHelper.If(isHitboxActive, DespawnHitbox);
			isHandlingAttack = false;

			animationHandler.UnbindEvents();
			eventDispatcher.onAttackStart -= source.behavior.OnAttackStart;
			eventDispatcher.onAttackEnd -= source.behavior.OnAttackEnd;
			eventDispatcher.onHitFrame -= OnHitFrame;
		}


		private void EnrollBehaviorContext() {
			if (!isHandlingAttack) {
				throw new InvalidOperationException("EnrollBehaviorContext called on a non-handled attack");
			}
			source.behavior.Enroll(this);
		}

		private void EndBehaviorContext() {
			if (!isHandlingAttack) {
				throw new InvalidOperationException("EndBehaviorContext called on a non-handled attack");
			}
			source.behavior.End();
		}

		private void OnHitFrame(Collider2D other) {
			if (other.TryGetComponent<Hurtbox>(out var hurtbox)) {
				IHitRecognizer hitRecognizer = source.behavior.GetHitRecognizer();

				if (hitRecognizer.ShouldRegisterHit(hurtbox)) {
					hurtbox.NotifyHit(source);
					source.behavior.OnHitRegistered(other);
				}
				hitRecognizer.OnHitFrame(hurtbox);
				source.behavior.OnHitFrame(other);
			} else {
				logger.LogError(null, "Could not find Hurtbox on hitbox collider: {}", other.gameObject);
			}
		}
	}
}
