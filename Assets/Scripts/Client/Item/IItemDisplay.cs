using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IItemDisplay {
		void SetStack(ItemStack itemStack);
		ItemStack GetStack();
	}
}
