using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public static class Attributes {
		public static readonly AttributeType<int> intType = Registry<AttributeType>.Add(new AttributeType<int>(new Identifier("int"), new NumberRange<int>(0, 100)));
		public static readonly AttributeType<float> floatType = Registry<AttributeType>.Add(new AttributeType<float>(new Identifier("float"), new NumberRange<float>(-10f, 10f)));
		public static readonly AttributeType<double> doubleType = Registry<AttributeType>.Add(new AttributeType<double>(new Identifier("double"), new NumberRange<double>(-1d, 1d)));
	}
}
