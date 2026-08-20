using SoulboundEngine.Loot.Context;
using System;

namespace SoulboundEngine.Loot.Provider.Number {
	public record BinomialLootNumberProvider(ILootNumberProvider trialProvider, ILootNumberProvider probabilityProvider) : ILootNumberProvider {
		public float NextFloat(LootContext context) => this.Roll(context);

		private int Roll(LootContext context) {
			int successes = 0;
			int trials = Math.Max(0, this.trialProvider.NextInt(context));
			float probability = Math.Clamp(this.probabilityProvider.NextFloat(context), 0f, 1f);

			for (int i = 0; i < trials; i++) {
				if (context.random.NextDouble() < probability) {
					successes++;
				}
			}

			return successes;
		}

		public static BinomialLootNumberProvider Create(int trials, float probability) {
			return new BinomialLootNumberProvider(ConstantLootNumberProvider.Create(trials), ConstantLootNumberProvider.Create(probability));
		}
	}
}
