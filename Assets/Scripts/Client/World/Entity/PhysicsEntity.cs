using SoulboundBackend.Common;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	[PROTOTYPICAL]
	public sealed class PhysicsEntity : Entity {
		private readonly AssetKey spriteKey = new("WhiteSquare");
		public PhysicsEntity(Vector2 initialPos)
			: base(EntityType.PHYSICS_ENTITY, initialPos) {
		}

		protected override IEntityTransform CreateTransform() {
			GameObject obj = new("Physics Entity", typeof(PhysicsTransform));

			Sprite sprite = AssetManager.Resolve<Sprite>(spriteKey);
			obj.AddComponent<SpriteRenderer>().sprite = sprite;
			obj.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
			obj.AddComponent<BoxCollider2D>();

			return obj.GetComponent<PhysicsTransform>();
		}
	}
}
