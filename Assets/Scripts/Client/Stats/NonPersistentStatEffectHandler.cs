using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class NonPersistentStatEffectHandler : IStatEffectHandler {
	protected IEnumerable<AbstractSerializableStat> usedStats;

	public NonPersistentStatEffectHandler(IEnumerable<AbstractSerializableStat> usedStats) {
		this.usedStats = usedStats.Select(s => {
			var copy = (AbstractSerializableStat)s.Clone();
			copy.persistent = false;
			return copy;
		});
	}

	public abstract void Disable(IStatSource source);
	public abstract void Enable(IStatSource source);

	public IEnumerable<AbstractSerializableStat> SuppliedStats() {
		return usedStats;
	}
}
