using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Client.Stats {
	[Obsolete]
	public sealed class StatMapping {
		public AbstractValueModifier stat { get; }
		public IReadOnlyList<StatActivator> activators { get; }

		public StatMapping(AbstractValueModifier stat, IEnumerable<StatActivator> activators) {
			this.stat = stat;
			this.activators = activators.ToList();
		}
	}
}
