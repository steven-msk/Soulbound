using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Stats {
	public interface IStatOwner {
		IReadOnlyDictionary<IStatDefinition, IStatEntry> GetEntries();

		bool TryGetEntry(IStatDefinition definition, out IStatEntry entry);
	}
}
