namespace SoulboundEngine.Core.Event {
	public interface IEventHandler<in T> : IEventListener<T> where T : struct, IGameEvent {

	}
}
