using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client {
	public abstract class TickManager {
		protected readonly List<ITickable> tickables = new();

		public void AddTickable(ITickable tickable) {
			this.tickables.Add(tickable);
		}

		public void RemoveTickable(ITickable tickable) {
			this.tickables.Remove(tickable);
		}

		public virtual void Tick() {
			foreach (var tickable in tickables) {
				tickable.Tick();
			}
		}
	}
}
