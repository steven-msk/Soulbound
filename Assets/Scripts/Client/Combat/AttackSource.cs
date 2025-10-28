using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.Combat {
	public class AttackSource {
		public readonly float baseDamage;
		public readonly float knockbackForce;
		public readonly Func<Hitbox> hitboxSupplier;
		public Action? onAttackStart { get; private set; }
		public Action? onAttackEnd { get; private set; }
		public AnimationEvent? onAttackAnimationStart { get; private set; }
		public AnimationEvent? onAttackAnimationEnd { get; private set; }
		public Action<Collider2D>? onHitRegistered { get; private set; }
		public Action<Collider2D>? onHitFrame { get; private set; }
		public Action<Hitbox>? onHitboxSpawned { get; private set; }
		public Action? onHitboxDespawned { get; private set; }
		public Action<Collider2D>? onHitboxEnter { get; private set; }
		public Action<Collider2D>? onHitboxExit { get; private set; }

		// hit immunity frames, hit registration method, validators...

		public AttackSource(float baseDamage, float knockbackForce, Func<Hitbox> hitboxSupplier) {
			this.baseDamage = baseDamage;
			this.knockbackForce = knockbackForce;
			this.hitboxSupplier = hitboxSupplier;
		}

		public class Builder {
			private AttackSource instance;

			public Builder(float baseDamage, float knockbackForce, Func<Hitbox> hitboxSupplier) {
				instance = new AttackSource(baseDamage, knockbackForce, hitboxSupplier);
			}

			public Builder OnAttackStart(Action action) {
				instance.onAttackStart = action;
				return this;
			}

			public Builder OnAttackEnd(Action action) {
				instance.onAttackEnd = action;
				return this;
			}

			public Builder OnAttackAnimationStart(AnimationEvent action) {
				instance.onAttackAnimationStart = action;
				return this;
			}

			public Builder OnAttackAnimationEnd(AnimationEvent action) {
				instance.onAttackAnimationEnd = action;
				return this;
			}

			public Builder OnHitRegistered(Action<Collider2D> action) {
				instance.onHitRegistered = action;
				return this;
			}

			public Builder OnHitFrame(Action<Collider2D> action) {
				instance.onHitFrame = action;
				return this;
			}

			public Builder OnHitboxSpawned(Action<Hitbox> action) {
				instance.onHitboxSpawned = action;
				return this;
			}

			public Builder OnHitboxDespawned(Action action) {
				instance.onHitboxDespawned = action;
				return this;
			}

			public Builder OnHitboxEnter(Action<Collider2D> action) {
				instance.onHitboxEnter = action;
				return this;
			}

			public Builder OnHitboxExit(Action<Collider2D> action) {
				instance.onHitboxExit = action;
				return this;
			}

			public AttackSource GetInstance() => instance;
		}
	}
}
