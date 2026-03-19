using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;
using SoulboundBackend.Client.Debug.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Client.Debug.Logging.Logger;
using SoulboundBackend.Client.World.EntitySystem.Transform;

namespace SoulboundBackend.Client.World.EntitySystem {
	[PROTOTYPICAL]
	public sealed class MovingEntity : Entity, ITickingEntity {
		private readonly AssetKey spriteKey = new("WhiteSquare");

		public MovingEntity(Vector2 initialPos)
			: base(EntityType.MOVING_ENTITY, initialPos) {
		}

		public void Tick() {
			SetPos(GetPos() + new Vector2(1f, 0f));
		}

		protected override IEntityTransform CreateTransform() {
			GameObject obj = new("Static Entity", typeof(StaticTransform));

			Sprite sprite = AssetManager.Resolve<Sprite>(spriteKey);
			obj.AddComponent<SpriteRenderer>().sprite = sprite;

			return obj.GetComponent<StaticTransform>();
		}

	}
}
