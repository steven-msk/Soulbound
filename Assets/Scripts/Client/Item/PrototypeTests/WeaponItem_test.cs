using SoulboundBackend.Client.Combat;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Tooltip;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class WeaponItem_test : Item, IAttackSourceProvider {
	public override string name => "weaponItem_test";

	public override ItemAspect aspect => ItemAspectRegistry.Get(this, () => ItemAspect.Simple("fruit_icon"));

	public override int maxStackSize => 1;

	protected override Func<Item, TooltipData> tooltipSupplier => null;

	protected override TooltipRenderer.NodeStyleProvider nodeStyleProvider => null;

	public bool GetAttackSource(ItemUseTrigger trigger, out AttackSource source) {
		if (trigger == ItemUseTrigger.LeftClick) {
			var animationPrefab = Resources.Load<GameObject>("weaponItem_test_hitbox");
			source = new AttackSource(10, 1, new TestBehavior(), context => {
				var parentObj = new GameObject("parent animation object");
				parentObj.transform.SetParent(context.performer.transform);
				parentObj.transform.position = new Vector2(1f * context.performer.facing, 1f) + context.performer.position;

				var obj = GameObject.Instantiate(animationPrefab, parentObj.transform, true);
				context.AddTemp(parentObj);
				return AttackAnimatorChannel.FromDelegates(
					obj.GetComponent<Animator>,
					obj.GetComponent<AttackEventDispatcher>
				);
			}, animator => animator.Play("attack"));
			return true;
		}
		source = default;
		return false;
	}

	class TestBehavior : IAttackBehavior {
		private IHitRecognizer hitRecognizer;
		private AttackHandler attackHandler;
		private Hitbox hitbox;

		public void End(AttackContext context) {
			UnityEngine.Debug.Log("test behavior ended");
			context.animationHandler.animatorReference.gameObject.GetComponent<Hitbox>().Deactivate();
			//hitbox.Deactivate();
			//GameObject.Destroy(hitbox.gameObject);
		}

		public void Enroll(AttackContext context, AttackHandler handler) {
			hitRecognizer = new OneTimeHitRecognizer();
			this.attackHandler = handler;
			UnityEngine.Debug.Log("test behavior enrolled");
			context.animationHandler.animatorReference.gameObject.GetComponent<Hitbox>().Activate(context.eventDispatcher);
			//hitbox = GameObject.Instantiate(Resources.Load<GameObject>("weaponItem_test_hitbox"), context.performer.transform, true)
			//	.GetComponent<Hitbox>();
			//hitbox.transform.position = new Vector2(context.performer.facing * 1f, 1f) + context.performer.position;
			//hitbox.Activate(context.eventDispatcher);
		}

		public IHitRecognizer GetHitRecognizer() {
			return hitRecognizer;
		}

		void IAttackBehavior.OnAttackAnimationEnd(AttackContext context) {
			attackHandler.EndAttack();
		}

		void IAttackBehavior.OnHitboxEnter(AttackContext context, Hitbox hitbox, Collider2D collider) {
			UnityEngine.Debug.Log("hitbox entered");
		}
	}
}
