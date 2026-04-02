using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public static class Attributes {
		public static readonly AttributeType<float> intType = Registry<AttributeType>.Add(new NumericAttribute(new Identifier("int"), new NumberRange(0, 100)));
		public static readonly AttributeType<float> floatType = Registry<AttributeType>.Add(new NumericAttribute(new Identifier("float"), new NumberRange(-10f, 10f)));
	}
}
