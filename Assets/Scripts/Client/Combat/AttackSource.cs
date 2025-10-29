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

		public AttackSource(float baseDamage, float knockbackForce, IAttackBehavior behavior) {
			this.baseDamage = baseDamage;
			this.knockbackForce = knockbackForce;
			this.behavior = behavior;
		}
	}
}
