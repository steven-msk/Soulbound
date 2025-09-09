using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class StatEffectHandler_test : IStatEffectHandler {
	private IStatProvider provider;
	private IEnumerable<AbstractSerializableStat> usedStats;

	public StatEffectHandler_test(IStatProvider provider, IEnumerable<AbstractSerializableStat> usedStats) {
		this.provider = provider;
		this.usedStats = usedStats;
	}

	public void Enable(IStatSource source) {
		source.ApplyStats(usedStats, provider);
	}

	public void Disable(IStatSource source) {
		source.RevokeStats(usedStats, provider);
	}

	public IEnumerable<AbstractSerializableStat> SuppliedStats() {
		return usedStats;
	}
}
