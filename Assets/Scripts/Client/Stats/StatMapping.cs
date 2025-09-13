using System.Collections.Generic;
using System.Linq;

public sealed class StatMapping {
	public AbstractSerializableStat stat { get; }
	public IReadOnlyList<StatActivator> activators { get; }

	public StatMapping(AbstractSerializableStat stat, IEnumerable<StatActivator> activators) {
		this.stat = stat;
		this.activators = activators.ToList();
	}
}
