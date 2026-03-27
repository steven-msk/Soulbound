using System;

namespace SoulboundEngine.Core.Event {
	public interface IHandlerWrapper : IListenerWrapper {
		Type GetHandlerType();
	}
}
