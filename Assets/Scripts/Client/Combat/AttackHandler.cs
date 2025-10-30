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
		private AttackContext context;
		private AttackAnimationHandler animationHandler;

		public AttackHandler(AttackSource source, AttackEventDispatcher eventDispatcher) {
			this.source = source;
			this.eventDispatcher = eventDispatcher;
		}

		public void StartAttack(AttackContext context, AttackAnimationHandler animationHandler) {
			if (isHandlingAttack) {
				throw new InvalidOperationException("Attempted to start attack when another has already begun");
			}
			this.context = context;
			this.animationHandler = animationHandler;
			isHandlingAttack = true;

			animationHandler.BindEvents(
				InjectContext_OnAttackAnimationStart,
				InjectContext_OnAttackAnimationEnd,
				EnrollBehaviorContext, 
				EndBehaviorContext
			);
			eventDispatcher.onAttackStart += InjectContext_OnAttackStart;
			eventDispatcher.onAttackEnd += InjectContext_OnAttackEnd;
			eventDispatcher.onHitFrame += OnHitFrame;

			eventDispatcher.OnAttackStart();
			animationHandler.StartAnimation(this);
		}

		public void EndAttack() {
			if (!isHandlingAttack) {
				throw new InvalidOperationException("Attempted to end attack when it hasnt even started yet");
			}
			eventDispatcher.OnAttackEnd();
			isHandlingAttack = false;

			animationHandler.UnbindEvents();
			eventDispatcher.onAttackStart -= InjectContext_OnAttackStart;
			eventDispatcher.onAttackEnd -= InjectContext_OnAttackEnd;
			eventDispatcher.onHitFrame -= OnHitFrame;
		}


		private void EnrollBehaviorContext() {
			if (!isHandlingAttack) {
				throw new InvalidOperationException("EnrollBehaviorContext called on a non-handled attack");
			}
			source.behavior.Enroll(context, this, eventDispatcher);
		}

		private void EndBehaviorContext() {
			if (!isHandlingAttack) {
				throw new InvalidOperationException("EndBehaviorContext called on a non-handled attack");
			}
			source.behavior.End(this.context);
		}

		private void OnHitFrame(Collider2D other) {
			if (other.TryGetComponent<Hurtbox>(out var hurtbox)) {
				IHitRecognizer hitRecognizer = source.behavior.GetHitRecognizer();

				if (hitRecognizer.ShouldRegisterHit(hurtbox)) {
					hurtbox.NotifyHit(source);
					source.behavior.OnHitRegistered(this.context, other);
				}
				hitRecognizer.OnHitFrame(hurtbox);
				source.behavior.OnHitFrame(this.context, other);
			} else {
				logger.LogError(null, "Could not find Hurtbox on hitbox collider: {}", other.gameObject);
			}
		}

		private void InjectContext_OnAttackAnimationStart() {
			source.behavior.OnAttackAnimationStart(this.context);
		}

		private void InjectContext_OnAttackAnimationEnd() {
			source.behavior.OnAttackAnimationEnd(this.context);
		}

		private void InjectContext_OnAttackStart() {
			source.behavior.OnAttackStart(this.context);
		}

		private void InjectContext_OnAttackEnd() {
			source.behavior.OnAttackEnd(this.context);
		}
	}
}
