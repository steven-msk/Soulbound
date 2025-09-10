using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class DelegatedStatEffectHandler : IStatEffectHandler {
	private IEnumerable<AbstractSerializableStat> suppliedStats;
	private Action<IStatSource> enableAction;
	private Action<IStatSource> disableAction;

	public DelegatedStatEffectHandler(Action<IStatSource> enableAction, Action<IStatSource> disableAction, IEnumerable<AbstractSerializableStat> suppliedStats) {
		this.enableAction = enableAction;
		this.disableAction = disableAction;
		this.suppliedStats = suppliedStats;
	}

	public void Enable(IStatSource source) {
		enableAction.Invoke(source);
	}

	public void Disable(IStatSource source) {
		disableAction.Invoke(source);
	}

	public IEnumerable<AbstractSerializableStat> SuppliedStats() {
		return suppliedStats;
	}
}
