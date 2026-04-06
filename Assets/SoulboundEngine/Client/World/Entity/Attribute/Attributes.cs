using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public static class Attributes {
		public static readonly AttributeType<float> intType = Create(new NumericAttribute(Identifier.Of("int"), new NumberRange(0, 100)));
		public static readonly AttributeType<float> floatType = Create(new NumericAttribute(Identifier.Of("float"), new NumberRange(-10f, 10f)));

		private static AttributeType<T> Create<T>(AttributeType<T> attribute) {
			return Registries.Register(Registries.ATTRIBUTES, attribute.GetIdentifier(), attribute);
		}

		public static void Init() { }
	}
}
