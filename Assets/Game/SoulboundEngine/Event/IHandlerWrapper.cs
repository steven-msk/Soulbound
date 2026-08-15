using System;

namespace SoulboundEngine.Event {
	public interface IHandlerWrapper : IListenerWrapper {
		Type GetHandlerType();
	}
}
