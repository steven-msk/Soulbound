using SoulboundEngine.Client.Loot.Context;

namespace SoulboundEngine.Client.Loot.Provider.Number {
	public record UniformLootNumberProvider(ILootNumberProvider min, ILootNumberProvider max) : ILootNumberProvider {
		public float NextFloat(LootContext context) {
			float min = this.min.NextFloat(context);
			float max = this.max.NextFloat(context);
			double t = context.random.NextDouble();
			return min + (max - min) * (float)t;
		}

		public int NextInt(LootContext context) {
			int min = this.min.NextInt(context);
			int max = this.max.NextInt(context);
			return context.random.NextInt(min, max + 1);
		}

		public static UniformLootNumberProvider Create(float min, float max) {
			return new UniformLootNumberProvider(ConstantLootNumberProvider.Create(min), ConstantLootNumberProvider.Create(max));
		}
	}
}
