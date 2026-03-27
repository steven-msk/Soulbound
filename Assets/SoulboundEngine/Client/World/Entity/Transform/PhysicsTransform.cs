using SoulboundEngine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem.Transform {
	[RequireComponent(typeof(Rigidbody2D))]
	public class PhysicsTransform : MonoBehaviour, IEntityTransform {
		private IEntityCollisionHandler collisionHandler;
		private Entity entity;
		private Rigidbody2D body;

		public void Bind(Entity entity) {
			this.entity = entity;
			body = GetComponent<Rigidbody2D>();
		}

		public void Destroy() => Destroy(gameObject);

		public Vector2 GetPos() => body.position;

		public void SetPos(Vector2 position) {
			body.position = position;
		}

		public Entity GetEntity() => entity;

		private void FixedUpdate() {
			entity.SetPos(body.position);
		}

		public void SetCollisionHandler(IEntityCollisionHandler collisionHandler) {
			this.collisionHandler = collisionHandler;
		}

		private void OnCollisionEnter2D(Collision2D collision) {
			collisionHandler?.OnCollisionEnter(new EntityCollision {
				self = entity,
				other = collision.otherCollider.GetComponent<IEntityTransform>()?.GetEntity(),
				point = collision.GetContact(0).point,
				normal = collision.GetContact(0).normal,
				otherObject = collision.otherCollider.gameObject
			});
		}

		private void OnCollisionExit2D(Collision2D collision) {
			collisionHandler?.OnCollisionExit(new EntityCollision {
				self = entity,
				other = collision.otherCollider.GetComponent<IEntityTransform>()?.GetEntity(),
				otherObject = collision.otherCollider.gameObject
			});
		}
	}
}
