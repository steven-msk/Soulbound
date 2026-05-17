using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.LevelDomain;

namespace SoulboundEngine.Client.Interaction {
	using Player = Player.Player;

	public struct ItemInteraction : IInteractionContext {
		// note: the implementation might change
		// as more features are introduced during prod

		public Player player;
		public Level level;
		public ItemStack itemStack;
		public InteractionTrigger trigger;

		public readonly Level GetLevel() => this.level;
		public readonly InteractionTrigger GetTrigger() => this.trigger;
	}
}
