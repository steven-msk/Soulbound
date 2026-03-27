using SoulboundEngine.Client.Interaction;

namespace SoulboundEngine.Client.World.BlockSystem {
	public interface IBlockInteractionListener {
		bool CanInteract(in BlockInteraction ctx);
		bool ValidateTrigger(InteractionTrigger trigger);
		void OnInteract(in BlockInteraction ctx);
	}
}
