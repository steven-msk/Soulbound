using SoulboundEngine.Common.Math.Random;

namespace SoulboundEngine.Loot.Context {
	public class LootContext {
		private readonly LootWorldContext worldContext;
		public IRandom random { get; }

		public LootContext(LootWorldContext worldContext, IRandom random) {
			this.random = random;
			this.worldContext = worldContext;
		}

		public float Luck => this.worldContext.luck;

	}
}
