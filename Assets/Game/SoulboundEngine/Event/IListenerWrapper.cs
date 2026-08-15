namespace SoulboundEngine.Event {
	public interface IListenerWrapper {
		object GetWrappedListener();
		void Fire(IGameEvent e);
	}
}
