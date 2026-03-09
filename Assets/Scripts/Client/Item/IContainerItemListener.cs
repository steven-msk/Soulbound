using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IContainerItemListener {
		void OnItemAdded(Item item, IItemContainer container);
		void OnItemRemoved(Item item, IItemContainer container);
	}
}
