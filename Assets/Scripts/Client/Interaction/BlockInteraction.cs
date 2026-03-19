using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Client.World.LevelDomain;

#nullable enable

namespace SoulboundBackend.Client.Interaction {
	public struct BlockInteraction : IInteractionContext {
		// note: the implementation might change
		// as more features are introduced during prod

		public Level level;
		public BlockPos blockPos;
		public BlockState blockState;
		public ItemStack? itemStack;
		public InteractionTrigger trigger;

		public readonly Level GetLevel() => level;
		public readonly InteractionTrigger GetTrigger() => trigger;
	}
}
