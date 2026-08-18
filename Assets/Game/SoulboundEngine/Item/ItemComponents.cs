namespace SoulboundEngine.Item {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Component;
	using SoulboundEngine.Registry;
	using System;

	public static class ItemComponents {
		public static readonly ComponentType<string> NAME = Register<string>("name", b => b.Codec(STRING_CODEC));
		public static readonly ComponentType<int> MAX_STACK_COUNT = Register<int>("max_stack_count", b => b.Codec(INT_CODEC));
		public static readonly ComponentType<int> BREAK_LEVEL = Register<int>("break_level", b => b.Codec(INT_CODEC));
		public static readonly ComponentType<int> DURABILITY = Register<int>("durability", b => b.Codec(INT_CODEC));

		public static readonly IComponentMap DEFAULT_COMPONENTS = IComponentMap.Create()
			.Add(MAX_STACK_COUNT, Item.DEFAULT_FULL_STACK)
			.Build();

		public static readonly ComponentType.Codec<int> INT_CODEC = GenericValueCodec(token => (int)token);
		public static readonly ComponentType.Codec<string> STRING_CODEC = GenericValueCodec(token => (string)token);

		private static ComponentType.Codec<T> GenericValueCodec<T>(Func<JToken, T> caster) => new(value => new JValue(value), caster);

		private static ComponentType<T> Register<T>(string id, Func<ComponentType<T>.Builder, ComponentType<T>.Builder> builder) {
			ComponentType<T> componentType = builder(ComponentType<T>.Create()).Build();
			Registry<ComponentType>.Register(Registries.COMPONENT_TYPE, id, componentType);
			return componentType;
		}
	}
}
