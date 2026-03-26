using System;

namespace SoulboundBackend.Core.Event {
	public interface IHandlerWrapper : IListenerWrapper {
		Type GetHandlerType();
	}
}
