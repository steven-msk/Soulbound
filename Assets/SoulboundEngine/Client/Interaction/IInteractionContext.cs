using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.LevelDomain;

namespace SoulboundEngine.Client.Interaction {
	public interface IInteractionContext {
		// TODO: find a proper use for IInteractionContext interface to avoid marking
		Level GetLevel();
		InteractionTrigger GetTrigger();
	}
}
