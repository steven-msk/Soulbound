using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.Combat {
	public interface IAttackBehavior {
		void Enroll(AttackHandler handler);
		void End();

		IHitRecognizer GetHitRecognizer();

		virtual void OnAttackStart() { }
		virtual void OnAttackEnd() { }
		virtual void OnAttackAnimationStart() { }
		virtual void OnAttackAnimationEnd() { }
		virtual void OnHitRegistered(Collider2D collider) { }
		virtual void OnHitFrame(Collider2D collider) { }
		virtual void OnHitboxEnter(Collider2D collider) { }
		virtual void OnHitbotExit(Collider2D collider) { }
	}
}
