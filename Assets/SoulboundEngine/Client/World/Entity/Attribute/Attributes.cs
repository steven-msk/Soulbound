using SoulboundEngine.Core;
using SoulboundEngine.Core.Registry;
using System.Collections.Generic;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public static class Attributes {
		public static readonly AttributeType<int> intType = Registry<AttributeType>.Add(new AttributeType<int>(new Identifier("int"), new NumberRange<int>(0, 100)));
		public static readonly AttributeType<float> floatType = Registry<AttributeType>.Add(new AttributeType<float>(new Identifier("float"), new NumberRange<float>(-10f, 10f)));
		public static readonly AttributeType<double> doubleType = Registry<AttributeType>.Add(new AttributeType<double>(new Identifier("double"), new NumberRange<double>(-1d, 1d)));
		public static readonly AttributeType<string> stringType = Registry<AttributeType>.Add(new AttributeType<string>(new Identifier("string"), new SetRange<string>("string1", "string2", "string3")));
		public static readonly AttributeType<char> charType = Registry<AttributeType>.Add(new AttributeType<char>(new Identifier("char"), new PredicateRange<char>(char.IsDigit)));
		public static readonly AttributeType<List<string>> listType = Registry<AttributeType>.Add(new AttributeType<List<string>>(new Identifier("list"), null));
		public static readonly AttributeType<Dictionary<string, string>> dictionaryType = Registry<AttributeType>.Add(new AttributeType<Dictionary<string, string>>(new Identifier("dictionary"), null));
	}
}
