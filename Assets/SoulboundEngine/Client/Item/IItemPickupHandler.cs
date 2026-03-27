using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.ItemSystem {
	public interface IItemPickupHandler {
		bool TryPickupStack(ItemStack itemStack);
	}
}
