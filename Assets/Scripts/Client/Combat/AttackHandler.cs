using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Client.Combat {
	public class AttackHandler {
		private readonly AttackSource source;
		public bool isHandlingAttack { get; private set; }
		private AttackContext context;
		private AttackEventDispatcher eventDispatcher;
		private AttackAnimationHandler animationHandler;

		public AttackHandler(AttackSource source) => this.source = source;
		
		public void StartAttack(Entity performer, object metadata) {
			AssertNotHandling();
			var context = new AttackContext(performer, source) {
				metadata = metadata
			};
			var animatorChannel = source.animatorChannelSupplier.Invoke(context);
			Animator animator = animatorChannel.animator;
			AttackEventDispatcher eventDispatcher = animatorChannel.eventDispatcher;
			var animationHandler = new AttackAnimationHandler(eventDispatcher, animator, source.initialAnimationTrigger);

			this.StartAttack(context, animationHandler, eventDispatcher);
		}

		public void StartAttack(AttackContext context, AttackAnimationHandler animationHandler, AttackEventDispatcher eventDispatcher) {
			AssertNotHandling();
			this.context = context;
			this.animationHandler = animationHandler;
			this.eventDispatcher = eventDispatcher;
			context.animationHandler ??= animationHandler;
			context.eventDispatcher ??= eventDispatcher;
			isHandlingAttack = true;

			animationHandler.BindEvents(
				InjectContext_OnAttackAnimationStart,
				InjectContext_OnAttackAnimationEnd,
				EnrollBehaviorContext, 
				EndBehaviorContext
			);
			eventDispatcher.onAttackStart += InjectContext_OnAttackStart;
			eventDispatcher.onAttackEnd += InjectContext_OnAttackEnd;
			eventDispatcher.onHitboxEnter += InjectContext_OnHitboxEnter;
			eventDispatcher.onHitboxExit += InjectContext_OnHitboxExit;
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
			eventDispatcher.onHitboxEnter -= InjectContext_OnHitboxEnter;
			eventDispatcher.onHitboxExit -= InjectContext_OnHitboxExit;
			eventDispatcher.onHitFrame -= OnHitFrame;

			context.DestroyAllTemp();
		}


		private void EnrollBehaviorContext() {
			if (!isHandlingAttack) {
				throw new InvalidOperationException("EnrollBehaviorContext called on a non-handled attack");
			}
			source.behavior.Enroll(context, this);
		}

		private void EndBehaviorContext() {
			if (!isHandlingAttack) {
				throw new InvalidOperationException("EndBehaviorContext called on a non-handled attack");
			}
			source.behavior.End(this.context);
		}

		private void OnHitFrame(Hitbox hitbox, Collider2D collider) {
			if (collider.TryGetComponent<Hurtbox>(out var hurtbox)) {
				IHitRecognizer hitRecognizer = source.behavior.GetHitRecognizer();

				if (hitRecognizer.ShouldRegisterHit(hurtbox)) {
					hurtbox.NotifyHit(source);
					source.behavior.OnHitRegistered(this.context, hitbox, collider);
				}
				hitRecognizer.OnHitFrame(hurtbox);
				source.behavior.OnHitFrame(this.context, hitbox, collider);
				return;
			}

			UnityEngine.Debug.Log(collider);
			
			if (collider.TryGetComponent<TilemapCollisionDetector>(out var tilemapCollisionDetector)) {
				tilemapCollisionDetector.NotifyHit(this.context, hitbox, collider);
				return;
			}

			Logger.LogError("Could not find Hurtbox on hitbox collider: {}", collider.gameObject);
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

		private void InjectContext_OnHitboxEnter(Hitbox hitbox, Collider2D collider) {
			source.behavior.OnHitboxEnter(this.context, hitbox, collider);
		}

		private void InjectContext_OnHitboxExit(Hitbox hitbox, Collider2D collider) {
			source.behavior.OnHitbotExit(this.context, hitbox, collider);
		}

		private void AssertNotHandling() {
			if (isHandlingAttack) {
				throw new InvalidOperationException("Attempted to start attack when another has already begun");
			}
		}
	}
}
