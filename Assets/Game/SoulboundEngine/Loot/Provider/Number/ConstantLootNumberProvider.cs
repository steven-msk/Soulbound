using SoulboundEngine.Loot.Context;

namespace SoulboundEngine.Loot.Provider.Number {
	public record ConstantLootNumberProvider(float value) : ILootNumberProvider {
		public float NextFloat(LootContext context) => this.value;

		public static ConstantLootNumberProvider Create(float value) => new(value);
	}
}
