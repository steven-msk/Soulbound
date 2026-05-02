using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem {
	[PROTOTYPICAL]
	public sealed class MovingEntity : Entity, ITickingEntity {
		public MovingEntity(EntityDescriptor<MovingEntity> descriptor, Level level)
			: base(descriptor, level) {
		}

		public void Tick() {
			SetPosition(GetPosition() + new Vector2(1f, 0f));
		}
	}
}
