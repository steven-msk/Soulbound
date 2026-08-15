using SoulboundEngine.Interaction;
using SoulboundEngine.World.Player;
using SoulboundEngine.Client.World;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Level;
using SoulboundEngine.Common;

namespace SoulboundEngine.Item {
	[PROTOTYPICAL]
	public sealed class TeleportPlayerItem : Item {
		public TeleportPlayerItem(Settings settings) : base(settings) {
		}

		public override IActionResult OnPrimaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) {
			if (level.GetBlock(blockPos) != Blocks.AIR) return IActionResult.PASS;

			player.SetPosition(blockPos.GetCenter());
			return IActionResult.SUCCESS;
		}
	}
}
