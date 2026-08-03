using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Entity;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.Item {
	[PROTOTYPICAL]
	public sealed class ChargeableItem : Item {
		public ChargeableItem(Settings settings) : base(settings) {
		}

		public override IActionResult OnPrimaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) {
			return IActionResult.SUCCESS;
		}

		public override int GetUseTime(ItemStack stack, InteractionType type, Level level, Entity user) {
			return 5;
		}

		public override ItemStack OnItemUsed(ItemStack stack, InteractionType type, Level level, Entity user) {
			Logger.LogInfo("Chargeable item used");
			return stack;
		}

		public override ItemStack OnUseCanceled(ItemStack stack, InteractionType type, Level level, Entity user, int remainingTicks) {
			Logger.LogInfo("Chargeable item use canceled");
			return stack;
		}

		public override ItemStack OnUseFinished(ItemStack stack, InteractionType type, Level level, Entity user) {
			Logger.LogInfo("Chargeable item use finished");
			return stack;
		}

		public override ItemStack OnUseTick(ItemStack stack, InteractionType type, Level level, Entity user, int remainingTicks) {
			Logger.LogInfo("Chargeable item use tick. remaining ticks: {}", remainingTicks);
			return stack;
		}

	}
}
