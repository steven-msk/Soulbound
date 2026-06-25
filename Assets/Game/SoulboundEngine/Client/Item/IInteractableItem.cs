using SoulboundEngine.Client.Interaction;

namespace SoulboundEngine.Client.Item {
	public interface IInteractableItem {
		bool ValidateTrigger(InteractionTrigger trigger);

		// pass in a context param if number of params grows
		bool CanExecute(ItemStack itemStack, in ItemInteraction ctx);
		bool TryExecute(ItemStack itemStack, in ItemInteraction ctx);
	}
}
