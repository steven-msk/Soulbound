using SoulboundEngine.World.Level;

namespace SoulboundEngine.Client.Loot.Context {
	public class LootWorldContext {
		public Level level { get; }
		public float luck { get; }

		public LootWorldContext(Level level, float luck) {
			this.level = level;
			this.luck = luck;
		}
	}
}
