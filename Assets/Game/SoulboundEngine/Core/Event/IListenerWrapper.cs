namespace SoulboundEngine.Core.Event {
	public interface IListenerWrapper {
		object GetWrappedListener();
		void Fire(IGameEvent e);
	}
}
