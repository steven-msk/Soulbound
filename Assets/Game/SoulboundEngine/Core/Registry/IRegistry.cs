using System.Collections.Generic;

namespace SoulboundEngine.Core.Registry {
	public interface IRegistry {
		bool ContainsId(Identifier id);
		HashSet<Identifier> GetIdentifiers();
		void Freeze();
	}
}
