using SoulboundEngine.World.Level;
using SoulboundEngine.Common;
using UnityEngine;

namespace SoulboundEngine.World.Entity {
	[PROTOTYPICAL]
	public sealed class MovingEntity : Entity, ITickingEntity {
		public MovingEntity(EntityDescriptor<MovingEntity> descriptor, Level.Level level)
			: base(descriptor, level) {
		}

		public void Tick() {
			SetPosition(GetPosition() + new Vector2(1f, 0f));
		}
	}
}
