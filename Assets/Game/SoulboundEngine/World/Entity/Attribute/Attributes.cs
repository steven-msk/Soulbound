namespace SoulboundEngine.World.Entity.Attribute {
	using SoulboundEngine.Registry;

	public static class Attributes {
		public static readonly RegistryEntry<AttributeType> SPEED = Ranged("speed", 0.0d, 0.0d, double.MaxValue);
		public static readonly RegistryEntry<AttributeType> GRAVITY = Ranged("gravity", 0.0d, 0.0d, double.MaxValue);
		public static readonly RegistryEntry<AttributeType> JUMP_POWER = Ranged("jump_power", 0.4d, 0.0d, double.MaxValue);
		public static readonly RegistryEntry<AttributeType> LUCK = Ranged("luck", 0.0d, double.MinValue, double.MaxValue);

		private static RegistryEntry<AttributeType> Ranged(string id, double defaultValue, double minValue, double maxValue) {
			return Register(id, new RangedAttribute(GetTranslationKey(id), defaultValue, minValue, maxValue));
		}

		private static RegistryEntry<AttributeType> Register(string id, AttributeType attribute) {
			return Registry<AttributeType>.RegisterEntry(Registries.ATTRIBUTE, Identifier.Of(id), attribute);
		}

		public static string GetTranslationKey(string id) {
			return Identifier.GetTranslationKey("attribute", id);
		}

		public static void Init() {
		}
	}
}
