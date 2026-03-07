using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Debug.Logging;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Client.World.EntitySystem {
	[PROTOTYPICAL]
	public sealed class PhysicsEntity : Entity, IEntityCollisionHandler {
		private readonly AssetKey spriteKey = new("WhiteSquare");
		private bool colliding = false;
		private PhysicsTransform physicsTransform;

		public PhysicsEntity(Vector2 initialPos)
			: base(EntityType.PHYSICS_ENTITY, initialPos) {
		}

		protected override IEntityTransform CreateTransform() {
			GameObject obj = new("Physics Entity", typeof(PhysicsTransform));

			Sprite sprite = AssetManager.Resolve<Sprite>(spriteKey);
			SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprite;
			spriteRenderer.color = GetSpriteColor();

			obj.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
			obj.AddComponent<BoxCollider2D>();

			physicsTransform = obj.GetComponent<PhysicsTransform>();
			physicsTransform.SetCollisionHandler(this);
			
			return physicsTransform;
		}

		private Color GetSpriteColor() {
			return colliding ? Color.red : Color.green;
		}

		public void OnCollisionEnter(EntityCollision collision) {
			colliding = true;
			physicsTransform.GetComponent<SpriteRenderer>().color = GetSpriteColor();
			Logger.LogInfo("collision enter: " + collision.other?.descriptor ?? "null");
		}

		public void OnCollisionExit(EntityCollision collision) {
			colliding = false;
			physicsTransform.GetComponent<SpriteRenderer>().color = GetSpriteColor();
		}
	}
}
