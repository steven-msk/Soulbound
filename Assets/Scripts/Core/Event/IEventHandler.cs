namespace SoulboundBackend.Core.Event {
	public interface IEventHandler<in T> : IEventListener<T> where T : struct, IGameEvent {

	}
}
