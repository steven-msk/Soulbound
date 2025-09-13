using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Client.Stats {
	public sealed class StatMapping {
		public AbstractSerializableStat stat { get; }
		public IReadOnlyList<StatActivator> activators { get; }

		public StatMapping(AbstractSerializableStat stat, IEnumerable<StatActivator> activators) {
			this.stat = stat;
			this.activators = activators.ToList();
		}
	}
}
