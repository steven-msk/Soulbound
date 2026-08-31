namespace SoulboundEngine.Item {
	using SoulboundEngine.Common;
	using SoulboundEngine.Interaction;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;

	[PROTOTYPICAL]
	public sealed class TeleportPlayerItem : Item {
		public TeleportPlayerItem(Settings settings) : base(settings) {
		}

		public override IActionResult OnPrimaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) {
			if (level.GetBlock(blockPos) != Blocks.AIR) return IActionResult.PASS;

			player.SetPos(blockPos.GetCenter());
			return IActionResult.SUCCESS.DamageItem(player, EquipmentSlot.MAIN_HAND);
		}
	}
}
