namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Level;

#nullable enable

	public sealed class Heightmap {
		public int planeY { get; private set; }
		public int planeHeight => Level.MAX_Y - this.planeY;

		public Heightmap(int planeY) {
			this.planeY = planeY;
		}

		public float SampleHeight(int blockX, BiomeWeight primary, BiomeWeight? secondary) {
			float w1 = primary.value;
			float w2 = secondary.GetValueOrDefault().value;
			float t = this.GetBlendFactor(w1, secondary != null ? w2 : 0f);

			TerrainModulation m1 = primary.biome.SampleTerrain(blockX);
			if (secondary == null) {
				return this.ApplyModulation(m1);
			}
			TerrainModulation m2 = secondary.Value.biome.SampleTerrain(blockX);

			float h1 = this.ApplyModulation(m1);
			float h2 = this.ApplyModulation(m2);
			float blended = (float)Maths.Lerp(h1, h2, t);

			return blended;
		}

		public float ApplyModulation(TerrainModulation m) {
			float baseHeight = this.planeHeight + m.heightOffset;
			float variation = (this.planeHeight * (m.amplitude - 1f));
			variation *= m.erosion;
			return baseHeight + variation;
		}

		private float GetBlendFactor(float a, float b) {
			float t = b / (a + b);
			return (float)Maths.SmoothStep(0f, 1f, t);
			//return b / (a + b);
		}


		public int ToHeightValue(int yCoord) {
			return Level.MAX_Y - yCoord;
		}

		public int ToYCoord(int heightValue) {
			return Level.MIN_Y + heightValue;
		}
	}
}
