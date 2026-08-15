using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.Level;

#nullable enable

namespace SoulboundEngine.Item {
	public class ItemUsageContext {
		public BlockPos blockPos { get; protected set; }
		public Level level { get; protected set; }
		public PlayerEntity? player { get; protected set; }
		public ItemStack stack { get; protected set; }

		public ItemUsageContext(PlayerEntity player, BlockPos blockPos)
			: this(player.GetLevel(), player, player.GetMainHandStack(), blockPos) {
		}

		protected ItemUsageContext(Level level, PlayerEntity? player, ItemStack stack, BlockPos blockPos) {
			this.level = level;
			this.player = player;
			this.stack = stack;
			this.blockPos = blockPos;
		}
	}
}
