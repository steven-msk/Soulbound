namespace SoulboundEngine.Registry {
	using System.Collections.Generic;

	public interface IRegistry {
		bool ContainsId(Identifier id);

		HashSet<Identifier> GetIdentifiers();

		void Freeze();

		Identifier GetKeyIdentifier();
	}
}
