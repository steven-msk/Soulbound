using SoulboundEngine.Client.Component;
using SoulboundEngine.Core.Registry;
using System;

namespace SoulboundEngine.Client.Item {
	public static class ItemComponents {
		public static readonly ComponentType<string> NAME = Register<string>("name");
		public static readonly ComponentType<int> MAX_STACK_COUNT = Register<int>("max_stack_count");
		public static readonly ComponentType<int> BREAK_LEVEL = Register<int>("break_level");
		public static readonly ComponentType<int> DURABILITY = Register<int>("durability");

		public static readonly IComponentMap DEFAULT_COMPONENTS = IComponentMap.Create()
			.Add(MAX_STACK_COUNT, Item.DEFAULT_FULL_STACK)
			.Build();

		private static ComponentType<T> Register<T>(string id) => Register<T>(id, t => t);

		private static ComponentType<T> Register<T>(string id, Func<ComponentType<T>, ComponentType<T>> builder) {
			ComponentType<T> componentType = builder(new ComponentType<T>());
			Registry<ComponentType>.Register(Registries.COMPONENT_TYPE, id, componentType);
			return componentType;
		}
	}
}
