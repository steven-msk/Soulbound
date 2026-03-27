using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulboundEngine.Client.Stats {
	[Obsolete]
	public abstract class NonPersistentStatEffectHandler : IStatEffectHandler {
		protected IEnumerable<AbstractValueModifier> usedStats;

		public NonPersistentStatEffectHandler(IEnumerable<AbstractValueModifier> usedStats) {
			this.usedStats = usedStats.Select(s => {
				var copy = (AbstractValueModifier)s.Clone();
				//copy.persistent = false;
				UnityEngine.Debug.Log("cloned to non-persistent: "+ copy.GetHashCode() + ": "+ copy);
				return copy;
			}).ToList();
		}

		public abstract void Disable(IStatReceiver receiver);
		public abstract void Enable(IStatReceiver receiver);

		public IEnumerable<AbstractValueModifier> SuppliedStats() {
			return usedStats;
		}
	}
}
