using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public interface IItemSlotHandle : IUIElementHandle {
		IItemContainer GetContainer();
		IItemSlot GetSlot();
	}
}
