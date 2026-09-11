namespace SoulboundEngine.World.Gen.Biome {
	public sealed class Biome {
		private readonly float shape;

		public Biome(float shape) {
			this.shape = shape;
		}

		public float Shape => this.shape;
	}
}
