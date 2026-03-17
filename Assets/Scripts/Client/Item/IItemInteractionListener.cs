using SoulboundBackend.Client.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.ItemSystem {
	public interface IItemInteractionListener {
		bool ValidateTrigger(InteractionTrigger trigger);

		// pass in a context param if number of params grows
		bool CanExecute(ItemStack itemStack, in ItemInteraction ctx);
		bool TryExecute(ItemStack itemStack, in ItemInteraction ctx);
	}
}
