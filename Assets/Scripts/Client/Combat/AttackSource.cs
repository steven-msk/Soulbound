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
		public readonly IAttackBehavior behavior;
		public readonly Func<AttackContext, AttackAnimatorChannel> animatorChannelSupplier;
		public readonly Action<Animator> initialAnimationTrigger;

		public AttackSource(
				float baseDamage,
				float knockbackForce,
				IAttackBehavior behavior, 
				Func<AttackContext, AttackAnimatorChannel> animatorChannelSupplier,
				Action<Animator> initialAnimationTrigger
			) {
			this.baseDamage = baseDamage;
			this.knockbackForce = knockbackForce;
			this.behavior = behavior;
			this.animatorChannelSupplier = animatorChannelSupplier;
			this.initialAnimationTrigger = initialAnimationTrigger;
		}
	}
}
