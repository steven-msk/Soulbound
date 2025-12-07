using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public sealed class EntityTickManager : TickManager, IEntitySubsystem {
		public void AddEntity(Entity entity) {
			if (entity is ITickable tickable) {
				this.AddTickable(tickable);
			}
		}

		public void RemoveEntity(Entity entity) {
			if (entity is ITickable tickable) {
				this.RemoveTickable(tickable);
			}
		}
	}
}
