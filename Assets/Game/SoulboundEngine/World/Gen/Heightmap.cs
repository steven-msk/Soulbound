using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.Gen {
	using Level = Level.Level;

	public sealed class Heightmap {
		public int planeY { get; private set; }
		public int planeHeight => Level.MAX_Y - this.planeY;

		public Heightmap(int planeY) {
			this.planeY = planeY;
		}

		public float SampleHeight(int blockX, BiomeWeight primary, BiomeWeight? secondary) {
			var w1 = primary.value;
			var w2 = secondary.GetValueOrDefault().value;
			float t = this.GetBlendFactor(w1, secondary != null ? w2 : 0f);

			var m1 = primary.biome.SampleTerrain(blockX);
			if (secondary == null) {
				return this.ApplyModulation(m1);
			}
			var m2 = secondary.Value.biome.SampleTerrain(blockX);

			var h1 = this.ApplyModulation(m1);
			var h2 = this.ApplyModulation(m2);
			var blended = Mathf.Lerp(h1, h2, t);

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
			return Mathf.SmoothStep(0f, 1f, t);
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
