using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Interaction;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.Entity;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Common;

namespace SoulboundEngine.Item {
	[PROTOTYPICAL]
	public sealed class DebugPointerItem : Item {
		public DebugPointerItem(Settings settings) : base(settings) {
		}

		public override IActionResult OnPrimaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) {
			level.TryGetEntityAt(player.GetWorldPointerPos(), out Entity entity);
			Logger.LogInfo("Pointer: {}, BlockPos: {}, BlockState: {}, Entity: {}",
				player.GetWorldPointerPos(), blockPos, level.GetBlockState(blockPos), entity);
			return IActionResult.SUCCESS;
		}
	}
}
