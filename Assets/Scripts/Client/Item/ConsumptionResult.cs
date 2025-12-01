#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public record ConsumptionResult(ItemStack stack, bool success) {
		public static ConsumptionResult Success(ItemStack stack) => new(stack, true);
		public static ConsumptionResult Fail(ItemStack stack) => new(stack, false);
	}
}
