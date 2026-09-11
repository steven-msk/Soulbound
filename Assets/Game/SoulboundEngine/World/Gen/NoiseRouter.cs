namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.World.Gen.Biome;

	public record NoiseRouter(IDensityFunction[] densityFunctions) {
		public NoiseRouter MapAll(IDensityFunction.IVisitor visitor) {
			IDensityFunction[] newFunctions = new IDensityFunction[this.densityFunctions.Length];
			for (int i = 0; i < this.densityFunctions.Length; i++) {
				newFunctions[i] = this.densityFunctions[i].MapAll(visitor);
			}
			return new NoiseRouter(newFunctions);
		}
	}
}
