namespace SoulboundBackend.Core.Event {
	public sealed class ListenerWrapper<T> : IListenerWrapper where T : struct, IGameEvent {
		private readonly IEventListener<T> listener;

		public ListenerWrapper(IEventListener<T> listener) {
			this.listener = listener;
		}

		public void Fire(IGameEvent e) => listener.OnEvent((T)e);
		public object GetWrappedListener() => listener;
	}
}
