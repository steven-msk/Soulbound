using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

public sealed class StatMapping {
	public AbstractSerializableStat stat { get; }
	public IReadOnlyList<StatActivator> activators { get; }

	public StatMapping(AbstractSerializableStat stat, IEnumerable<StatActivator> activators) {
		this.stat = stat;
		this.activators = activators.ToList();
	}
}
