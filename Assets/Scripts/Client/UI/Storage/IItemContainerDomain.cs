using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI.Storage {
	public interface IItemContainerDomain {
		IItemSlot GetSlot(int index);
	}
}
