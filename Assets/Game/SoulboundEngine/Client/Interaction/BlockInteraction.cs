using SoulboundEngine.Client.Item;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Level;

#nullable enable

namespace SoulboundEngine.Client.Interaction {
	using PlayerEntity = Player.PlayerEntity;

	public struct BlockInteraction : IInteractionContext {
		// note: the implementation might change
		// as more features are introduced during prod

		public Level level;
		public BlockPos blockPos;
		public BlockState blockState;
		public ItemStack? itemStack;
		public InteractionTrigger trigger;
		public PlayerEntity? player;

		public readonly Level GetLevel() => this.level;
		public readonly InteractionTrigger GetTrigger() => this.trigger;
	}
}
