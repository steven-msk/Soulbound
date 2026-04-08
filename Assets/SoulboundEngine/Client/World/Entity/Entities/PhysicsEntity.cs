using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem {
	[PROTOTYPICAL]
	public sealed class PhysicsEntity : Entity, IEntityCollisionHandler {
		private readonly AssetKey spriteKey = new("WhiteSquare");
		private bool colliding = false;
		private PhysicsTransform physicsTransform;

		public PhysicsEntity(Level level)
			: base(EntityType.PHYSICS_ENTITY, level) {
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
		}

		public void OnCollisionExit(EntityCollision collision) {
			colliding = false;
			physicsTransform.GetComponent<SpriteRenderer>().color = GetSpriteColor();
		}
	}
}
