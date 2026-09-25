namespace SoulboundEngine.Loot.Context {
	using SoulboundEngine.World.Level;

	public class LootWorldContext {
		public Level level { get; }
		public float luck { get; }

		public LootWorldContext(Level level, float luck) {
			this.level = level;
			this.luck = luck;
		}

		public sealed class Builder {
			public Level level { get; }
			public float luck { get; private set; }

			public Builder(Level level) {
				this.level = level;
			}

			public Builder Luck(float luck) {
				this.luck = luck;
				return this;
			}

			public LootWorldContext Build() {
				return new LootWorldContext(this.level, this.luck);
			}
		}
	}
}
