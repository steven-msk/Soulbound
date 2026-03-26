using System;
using System.Collections.Generic;

namespace SoulboundBackend.Core.Event {
	public class EventDispatcher {
		private IGameEvent e;
		public event Action onDispatchFinished;

		public EventDispatcher(IGameEvent e) {
			this.e = e;
		}

		public void Dispatch(IEnumerable<IListenerWrapper> listeners) {
			foreach (var listener in listeners) {
				listener.Fire(e);
			}
			onDispatchFinished?.Invoke();
		}
	}
}
