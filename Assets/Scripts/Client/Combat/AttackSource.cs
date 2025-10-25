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
		public Action? onAttackStart { get; private set; } = null!;
		public Action? onAttackEnd { get; private set; } = null!;
		public AnimationEvent? onAttackAnimationStart { get; private set; } = null!;
		public AnimationEvent? onAttackAnimationEnd { get; private set; } = null!;
		public Action<Collider2D>? onHit { get; private set; } = null!;
		public Action<Hitbox>? onHitboxSpawned { get; private set; } = null!;
		public Action? onHitboxDespawned { get; private set; } = null!;

		// hit immunity frames, hit registration method, validators...

		public AttackSource(float baseDamage, float knockbackForce) {
			this.baseDamage = baseDamage;
			this.knockbackForce = knockbackForce;
		}

		public class Builder {
			private AttackSource instance;

			public Builder(float baseDamage, float knockbackForce) {
				instance = new AttackSource(baseDamage, knockbackForce);
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

			public Builder OnHit(Action<Collider2D> action) {
				instance.onHit = action;
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

			public AttackSource GetInstance() => instance;
		}
	}
}
