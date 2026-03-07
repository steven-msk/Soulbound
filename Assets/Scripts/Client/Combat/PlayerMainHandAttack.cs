using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.Combat {
	[Obsolete]
	public class PlayerMainHandAttack : IAttackBehavior {
		private AttackHandler attackHandler = null!;
		private IHitRecognizer hitRecognizer = null!;

		public void Enroll(AttackContext context, AttackHandler handler) {
			this.attackHandler = handler;
			this.hitRecognizer = new OneTimeHitRecognizer();
			//context.performer.GetComponent<Hitbox>().Activate(context.eventDispatcher);
		}

		public void End(AttackContext context) {
			//context.performer.GetComponent<Hitbox>().Deactivate();
		}

		void IAttackBehavior.OnAttackAnimationEnd(AttackContext context) {
			attackHandler.EndAttack();
		}

		public IHitRecognizer GetHitRecognizer() => hitRecognizer;
	}
}
