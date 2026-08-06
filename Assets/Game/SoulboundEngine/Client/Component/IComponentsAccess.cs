#nullable enable

namespace SoulboundEngine.Client.Component {
	public interface IComponentsAccess {
		T Get<T>(ComponentType<T> type);
		T GetOrDefault<T>(ComponentType<T> type, T fallback);
	}
}
