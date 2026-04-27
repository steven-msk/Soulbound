using System.Collections.Generic;

namespace SoulboundEngine.Client.Input {
	public interface IInputEventHandler {
		public virtual int priority => 0;
		IEnumerable<InputEventListener> GetListeners();
	}
}
