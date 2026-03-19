using SoulboundBackend.Client.World.EntitySystem.Transform;
using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	[PROTOTYPICAL]
	public sealed class StaticEntity : Entity {
		private readonly AssetKey spriteKey = new("WhiteSquare");

		public StaticEntity(Vector2 initialPos)
			: base(EntityType.STATIC_ENTITY, initialPos) {
		}

		protected override IEntityTransform CreateTransform() {
			GameObject obj = new("Static Entity", typeof(StaticTransform));

			Sprite sprite = AssetManager.Resolve<Sprite>(spriteKey);
			obj.AddComponent<SpriteRenderer>().sprite = sprite;

			return obj.GetComponent<StaticTransform>();
		}
	}
}
