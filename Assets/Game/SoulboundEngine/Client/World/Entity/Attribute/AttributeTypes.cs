using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public static class AttributeTypes {
		public static readonly RegistryEntry<EntityAttribute> attribute = Register("attribute", new EntityAttribute(IValueRule.Ranged(-10d, 10d), 1d));

		private static RegistryEntry<EntityAttribute> Register(string id, EntityAttribute attribute) {
			return Registry<EntityAttribute>.RegisterEntry(Registries.ATTRIBUTES, id, attribute);
		}

		public static void Init() { }
	}
}
