using SoulboundEngine.Client.Interaction;

namespace SoulboundEngine.Client.World.BlockSystem {
	public interface IInteractableBlock {
		bool CanInteract(in BlockInteraction ctx);
		bool ValidateTrigger(InteractionTrigger trigger);
		void OnInteract(in BlockInteraction ctx);
	}
}
