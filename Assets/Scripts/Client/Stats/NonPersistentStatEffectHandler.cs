using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class NonPersistentStatEffectHandler : IStatEffectHandler {
	protected IEnumerable<AbstractSerializableStat> usedStats;

	public NonPersistentStatEffectHandler(IEnumerable<AbstractSerializableStat> usedStats) {
		this.usedStats = usedStats.Select(s => {
			var copy = (AbstractSerializableStat)s.Clone();
			copy.persistent = false;
			Debug.Log("cloned to non-persistent: "+ copy.GetHashCode() + ": "+ copy);
			return copy;
		}).ToList();
	}

	public abstract void Disable(IStatReceiver receiver);
	public abstract void Enable(IStatReceiver receiver);

	public IEnumerable<AbstractSerializableStat> SuppliedStats() {
		return usedStats;
	}
}
