using SoulboundEngine.Client.Interaction;

namespace SoulboundEngine.Client.Item {
	public interface IInteractableItem {
		bool ValidateTrigger(InteractionTrigger trigger);

		// pass in a context param if number of params grows
		bool CanExecute(in ItemStack itemStack, in ItemInteraction ctx);
		bool TryExecute(ref ItemStack itemStack, in ItemInteraction ctx);
	}
}
