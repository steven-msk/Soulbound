using SoulboundBackend.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client {
	public interface IInventoryExecutionService {
		void SetStack(int slotIndex, ItemStack? stack);
	}
}
