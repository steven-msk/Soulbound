using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.LevelDomain;

#nullable enable

namespace SoulboundEngine.Client.Interaction {
	using Player = Player.Player;

	public struct BlockInteraction : IInteractionContext {
		// note: the implementation might change
		// as more features are introduced during prod

		public Level level;
		public BlockPos blockPos;
		public BlockState blockState;
		public ItemStack? itemStack;
		public InteractionTrigger trigger;
		public Player? player;

		public readonly Level GetLevel() => this.level;
		public readonly InteractionTrigger GetTrigger() => this.trigger;
	}
}
