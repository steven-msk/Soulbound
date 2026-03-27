using SoulboundEngine.Client.ItemSystem;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.Stats {
	[Obsolete]
	public interface IStatReceiver {

		public void ApplyStats(IEnumerable<AbstractValueModifier> stats, IStatProvider provider);

		public void RevokeStats(IEnumerable<AbstractValueModifier> stats, IStatProvider provider);
	}
}
