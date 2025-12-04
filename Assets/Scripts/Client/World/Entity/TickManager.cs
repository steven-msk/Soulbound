using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.EntitySystem {
	public sealed class TickManager : IEntitySubsystem {
		private List<ITickable> tickables = new();

		public void AddEntity(Entity entity) {
			if (entity is ITickable tickable) {
				tickables.Add(tickable);
			}
		}

		public void RemoveEntity(Entity entity) {
			if (entity is ITickable tickable) {
				tickables.Remove(tickable);
			}
		}

		public void Tick() {
			foreach (var tickable in tickables) {
				tickable.Tick();
			}
		}
	}
}
