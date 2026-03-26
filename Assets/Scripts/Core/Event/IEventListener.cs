namespace SoulboundBackend.Core.Event {
	public interface IEventListener<in T> where T : struct, IGameEvent {
		void OnEvent(T e);
	}
}
