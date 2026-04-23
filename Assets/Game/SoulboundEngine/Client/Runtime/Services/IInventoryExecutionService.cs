using SoulboundEngine.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundEngine.Client.Runtime.Services {
	public interface IInventoryExecutionService {
		void SetStack(int slotIndex, ItemStack? stack);
	}
}
