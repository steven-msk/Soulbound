using SoulboundEngine.Client.Stats;
using SoulboundEngine.Common;

using SoulboundEngine.Client.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SoulboundEngine.Client.ItemSystem {
	[Obsolete]
	public interface IStatProvider {
		public IEnumerable<StatMapping> statMappings { get; }

		protected HashSet<StatActivator> GetActivators() {
			return statMappings.SelectMany(sm => sm.activators).Distinct().ToHashSet();
		}

		public IEnumerable<AbstractValueModifier> GetStats() {
			return statMappings.Select(sm => sm.stat);
		}
	}
}
