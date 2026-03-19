using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.LevelDomain;

namespace SoulboundBackend.Client.Interaction {
	public interface IInteractionContext {
		// TODO: find a proper use for IInteractionContext interface to avoid marking
		Level GetLevel();
		InteractionTrigger GetTrigger();
	}
}
