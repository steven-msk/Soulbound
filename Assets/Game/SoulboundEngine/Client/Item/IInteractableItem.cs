using SoulboundEngine.Client.Interaction;
using System;

namespace SoulboundEngine.Client.Item {
	[Obsolete]
	public interface IInteractableItem {
		bool ValidateTrigger(InteractionTrigger trigger);

		// pass in a context param if number of params grows
		bool CanExecute(in ItemStack itemStack, in ItemInteraction ctx);
		bool TryExecute(ref ItemStack itemStack, in ItemInteraction ctx);
	}
}
