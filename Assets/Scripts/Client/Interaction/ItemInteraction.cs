using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.Players;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.LevelDomain;

namespace SoulboundBackend.Client.Interaction {
	public struct ItemInteraction  : IInteractionContext {
		// note: the implementation might change
		// as more features are introduced during prod

		public Player player;
		public Level level;
		public ItemStack itemStack;
		public InteractionTrigger trigger;

		public readonly Level GetLevel() => level;
		public readonly InteractionTrigger GetTrigger() => trigger;
	}
}
