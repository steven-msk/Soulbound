namespace SoulboundEngine.Item {
	using SoulboundEngine.Component;
	using SoulboundEngine.Registry;
	using SoulboundEngine.Serialization;
	using System;

#nullable enable

	public static class ItemComponents {
		public static readonly ComponentType<string> NAME = Register<string>("name", b => b.Codec(Codecs.STRING));
		public static readonly ComponentType<int> MAX_STACK_COUNT = Register<int>("max_stack_count", b => b.Codec(Codecs.INT));
		public static readonly ComponentType<int> BREAK_LEVEL = Register<int>("break_level", b => b.Codec(Codecs.INT));
		public static readonly ComponentType<int> DURABILITY = Register<int>("durability", b => b.Codec(Codecs.INT));
		public static readonly ComponentType<ItemAttributeModifiers> ATTRIBUTE_MODIFIERS = Register<ItemAttributeModifiers>("attributes", b => b.Codec(ItemAttributeModifiers.CODEC));

		public static readonly IComponentMap DEFAULT_COMPONENTS = IComponentMap.Create()
			.Add(MAX_STACK_COUNT, Item.DEFAULT_FULL_STACK)
			.Build();

		private static ComponentType<T> Register<T>(string id, Func<ComponentType<T>.Builder, ComponentType<T>.Builder> builder) {
			RegistryKey<ComponentType> key = RegistryKey<ComponentType>.Of(Registries.COMPONENT_TYPE.GetKey(), Identifier.Of(id));
			ComponentType<T> componentType = builder(ComponentType<T>.Create(key)).Build();
			return Registry<ComponentType>.Register(Registries.COMPONENT_TYPE, key, componentType);
		}
	}
}
