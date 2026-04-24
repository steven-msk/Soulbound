using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem {
	[PROTOTYPICAL]
	public sealed class PhysicsEntity : Entity, IEntityCollisionHandler {
		private bool colliding = false;
		private PhysicsTransform physicsTransform;

		public PhysicsEntity(EntityDescriptor<PhysicsEntity> descriptor, Level level)
			: base(descriptor, level) {
		}

		public Color GetSpriteColor() {
			return colliding ? Color.red : Color.green;
		}

		protected override void OnTransformCreated(IEntityTransform transform) {
			this.physicsTransform = (PhysicsTransform)transform;
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
