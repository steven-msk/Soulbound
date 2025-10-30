using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.Combat {
	public interface IAttackBehavior {
		void Enroll(AttackContext context, AttackHandler handler, AttackEventDispatcher eventDispatcher);
		void End(AttackContext context);

		IHitRecognizer GetHitRecognizer();

		virtual void OnAttackStart(AttackContext context) { }
		virtual void OnAttackEnd(AttackContext context) { }
		virtual void OnAttackAnimationStart(AttackContext context) { }
		virtual void OnAttackAnimationEnd(AttackContext context) { }
		virtual void OnHitRegistered(AttackContext context, Collider2D collider) { }
		virtual void OnHitFrame(AttackContext context, Collider2D collider) { }
		virtual void OnHitboxEnter(AttackContext context, Collider2D collider) { }
		virtual void OnHitbotExit(AttackContext context, Collider2D collider) { }
	}
}
