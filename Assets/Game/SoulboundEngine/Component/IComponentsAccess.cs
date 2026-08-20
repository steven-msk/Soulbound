#nullable enable

namespace SoulboundEngine.Component {
	public interface IComponentsAccess {
		T Get<T>(ComponentType<T> type);
		T GetOrDefault<T>(ComponentType<T> type, T fallback);
	}
}
