using SoulboundEngine.Client.Interaction;

namespace SoulboundEngine.Client.ItemSystem {
	public interface IItemInteractionListener {
		bool ValidateTrigger(InteractionTrigger trigger);

		// pass in a context param if number of params grows
		bool CanExecute(ItemStack itemStack, in ItemInteraction ctx);
		bool TryExecute(ItemStack itemStack, in ItemInteraction ctx);
	}
}
