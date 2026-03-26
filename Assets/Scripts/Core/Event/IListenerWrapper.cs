namespace SoulboundBackend.Core.Event {
	public interface IListenerWrapper {
		object GetWrappedListener();
		void Fire(IGameEvent e);
	}
}
