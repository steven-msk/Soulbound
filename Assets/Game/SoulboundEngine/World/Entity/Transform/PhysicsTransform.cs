using System;
using UnityEngine;

namespace SoulboundEngine.World.Entity.Transform {
	[RequireComponent(typeof(Rigidbody2D))]
	[Obsolete]
	public class PhysicsTransform : MonoBehaviour, IEntityTransform {
		// currently the transform leaves the implementation hidden for physics transforms.
		// this encapsulation doesnt match the default way of entities to express their state.
		// so PlayerTransform, PhysicsTransform and StaticTransform are obsolete because of this.

		private IEntityCollisionHandler collisionHandler;
		private Entity entity;
		private Rigidbody2D body;

		public void Bind(Entity entity) {
			this.entity = entity;
			this.body = this.GetComponent<Rigidbody2D>();
		}

		public void Destroy() => Destroy(this.gameObject);

		public Vector2 GetPos() => this.body.position;

		public void SetPos(Vector2 position) {
			this.body.position = position;
		}

		public Entity GetEntity() => this.entity;

		[Obsolete]
		private void FixedUpdate() {
			//entity.SetPosition(body.position);
		}

		public void SetCollisionHandler(IEntityCollisionHandler collisionHandler) {
			this.collisionHandler = collisionHandler;
		}

		private void OnCollisionEnter2D(Collision2D collision) {
			this.collisionHandler?.OnCollisionEnter(new EntityCollision {
				self = this.entity,
				other = collision.otherCollider.GetComponent<IEntityTransform>()?.GetEntity(),
				point = collision.GetContact(0).point,
				normal = collision.GetContact(0).normal,
				otherObject = collision.otherCollider.gameObject
			});
		}

		private void OnCollisionExit2D(Collision2D collision) {
			this.collisionHandler?.OnCollisionExit(new EntityCollision {
				self = this.entity,
				other = collision.otherCollider.GetComponent<IEntityTransform>()?.GetEntity(),
				otherObject = collision.otherCollider.gameObject
			});
		}

		void IEntityTransform.FrameUpdate() {
		}
	}
}
