using System.Collections.Generic;

namespace SoulboundEngine.Registry {
	public interface IRegistry {
		bool ContainsId(Identifier id);
		HashSet<Identifier> GetIdentifiers();
		void Freeze();
	}
}
