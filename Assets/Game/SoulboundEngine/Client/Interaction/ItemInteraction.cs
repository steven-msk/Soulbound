using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.World.Level;

namespace SoulboundEngine.Client.Interaction {
	using PlayerEntity = Player.PlayerEntity;

	public struct ItemInteraction : IInteractionContext {
		// note: the implementation might change
		// as more features are introduced during prod

		public PlayerEntity player;
		public Level level;
		public ItemStack itemStack;
		public InteractionTrigger trigger;

		public readonly Level GetLevel() => this.level;
		public readonly InteractionTrigger GetTrigger() => this.trigger;
	}
}
