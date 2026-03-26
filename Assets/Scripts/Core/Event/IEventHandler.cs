namespace SoulboundBackend.Core.Event {
	public interface IEventHandler<T> : IEventListener<T> where T : struct, IGameEvent {

	}
}
