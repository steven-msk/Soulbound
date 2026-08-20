namespace SoulboundEngine.World.Gen {
	public struct TerrainModulation {
		public float heightOffset;
		public float amplitude;
		public float erosion;

		public TerrainModulation(float heightOffset, float amplitude, float erosion) {
			this.heightOffset = heightOffset;
			this.amplitude = amplitude;
			this.erosion = erosion;
		}
	}
}
