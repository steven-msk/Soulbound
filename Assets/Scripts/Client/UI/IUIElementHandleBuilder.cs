using SoulboundBackend.Client.UI.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public interface IUIElementHandleBuilder<THandle> where THandle : IUIElementHandle {
		THandle Build(IUIElementContainer container);
	}
}
