namespace SoulboundEngine.Client.Component {
	public interface IComponentHolder : IComponentsAccess {
		IComponentMap GetComponents();

		T IComponentsAccess.Get<T>(ComponentType<T> type) {
			return this.GetComponents().Get(type);
		}

		T IComponentsAccess.GetOrDefault<T>(ComponentType<T> type, T fallback) {
			return this.GetComponents().GetOrDefault(type, fallback);
		}
	}

	public static class ComponentHolderExtensions { 
		public static bool Contains(this IComponentHolder componentHolder, ComponentType type) {
			return componentHolder.GetComponents().Contains(type);
		}

		public static T Get<T>(this IComponentHolder componentHolder, ComponentType<T> type) {
			return componentHolder.Get(type);
		}

		public static T GetOrDefault<T>(this IComponentHolder componentHolder, ComponentType<T> type, T fallback) {
			return componentHolder.GetOrDefault(type, fallback);
		}
	}
}
