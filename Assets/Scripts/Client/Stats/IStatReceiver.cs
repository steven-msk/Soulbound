using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	[Obsolete]
	public interface IStatReceiver {

		public void ApplyStats(IEnumerable<AbstractValueModifier> stats, IStatProvider provider);

		public void RevokeStats(IEnumerable<AbstractValueModifier> stats, IStatProvider provider);
	}
}
