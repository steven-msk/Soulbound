using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client {
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
