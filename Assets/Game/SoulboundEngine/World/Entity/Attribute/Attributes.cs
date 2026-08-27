namespace SoulboundEngine.World.Entity.Attribute {
	using SoulboundEngine.Registry;

	public static class Attributes {
		public static readonly RegistryEntry<Attribute> ATTRIBUTE = Ranged("attribute", 0.0d, -10.0d, 10.0d);

		private static RegistryEntry<Attribute> Ranged(string id, double defaultValue, double minValue, double maxValue) {
			return Register(id, new RangedAttribute(GetTranslationKey(id), defaultValue, minValue, maxValue));
		}

		private static RegistryEntry<Attribute> Register(string id, Attribute attribute) {
			return Registry<Attribute>.RegisterEntry(Registries.ATTRIBUTE, Identifier.Of(id), attribute);
		}

		public static string GetTranslationKey(string id) {
			return Identifier.GetTranslationKey("attribute", id);
		}

		public static void Init() {
		}
	}
}
