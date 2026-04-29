namespace SoulboundEngine.Client.World {
	public readonly struct WorldSave {
		public readonly string name;
		public readonly int seed;

		public WorldSave(string name, int seed) {
			this.name = name;
			this.seed = seed;
		}
	}
}
