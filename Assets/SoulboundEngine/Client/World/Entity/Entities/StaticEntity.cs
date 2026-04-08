using SoulboundEngine.Client.World.EntitySystem.Transform;
using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem {
	[PROTOTYPICAL]
	public sealed class StaticEntity : Entity {
		private readonly AssetKey spriteKey = new("WhiteSquare");

		public StaticEntity(Level level)
			: base(EntityType.STATIC_ENTITY, level) {
		}

		protected override IEntityTransform CreateTransform() {
			GameObject obj = new("Static Entity", typeof(StaticTransform));

			Sprite sprite = AssetManager.Resolve<Sprite>(spriteKey);
			obj.AddComponent<SpriteRenderer>().sprite = sprite;

			return obj.GetComponent<StaticTransform>();
		}
	}
}
