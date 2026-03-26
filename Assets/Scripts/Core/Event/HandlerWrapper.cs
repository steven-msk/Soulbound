using System;

namespace SoulboundBackend.Core.Event {
	public sealed class HandlerWrapper<T> : IHandlerWrapper where T : struct, IGameEvent {
		private readonly IEventHandler<T> handler;

		public HandlerWrapper(IEventHandler<T> handler) {
			this.handler = handler;
		}

		public void Fire(IGameEvent e) => handler.OnEvent((T)e);

		public Type GetHandlerType() => handler.GetType();

		public object GetWrappedListener() => handler;
	}
}
