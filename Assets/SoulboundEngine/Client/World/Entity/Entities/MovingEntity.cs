using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Client.Debug.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;
using SoulboundEngine.Client.World.EntitySystem.Transform;

namespace SoulboundEngine.Client.World.EntitySystem {
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
