using SoulboundEngine.Client.Interaction;
using System;

namespace SoulboundEngine.Client.World.Block {
	[Obsolete]
	public interface IInteractableBlock {
		bool CanInteract(in BlockInteraction ctx);
		bool ValidateTrigger(InteractionTrigger trigger);
		void OnInteract(in BlockInteraction ctx);
	}
}
