using SoulboundBackend.Client.ItemSystem;
using System.Collections.Generic;

#nullable enable

namespace SoulboundBackend.Client.Stats {
	public interface IStatReceiver {

		public void ApplyStats(IEnumerable<AbstractSerializableStat> stats, IStatProvider provider);

		public void RevokeStats(IEnumerable<AbstractSerializableStat> stats, IStatProvider provider);
	}
}
