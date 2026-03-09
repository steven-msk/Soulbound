using SoulboundBackend.Client.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IItemAction {
		bool ValidateTrigger(ItemActionTrigger trigger);

		// pass in a context param if number of params grows
		bool CanExecute(ItemStack itemStack, Player player, Level level);
		bool TryExecute(ItemStack itemStack, Player player, Level level);
	}
}
