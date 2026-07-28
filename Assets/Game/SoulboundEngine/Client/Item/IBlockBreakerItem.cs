using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Item {

	// might be renamed to something more appropiate
	public interface IBlockBreakerItem {
		int GetBreakLevel(ItemStack itemStack);
	}
}
