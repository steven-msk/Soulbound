using SoulboundEngine.Client.Interaction;
using System;

namespace SoulboundEngine.Client.World.Block {
	[Obsolete]
	public interface IInteractableBlock {
		bool CanInteract(in BlockInteractionResult ctx);
		bool ValidateTrigger(InteractionTrigger trigger);
		void OnInteract(in BlockInteractionResult ctx);
	}
}
