using Assets.Scripts.Client.World.Biome;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.World.Generation {
	public sealed class Heightmap {
		const float HEIGHT_OFFSET_AMPLIFIER = 5f;

		public int planeY { get; private set; }
		public int planeHeight => WorldChunk.maxY - planeY;
		private PerlinNoise continentalNoise;

		public Heightmap(int seed, int planeY) {
			this.planeY = planeY;

			this.continentalNoise = new PerlinNoise(0, seed, frequency: 0.001f, amplitude: 80f);
		}

		public float SamplePlaneHeightOffset(int blockX) {
			return continentalNoise.Sample1D(blockX);
		}

		public float SampleHeight(int blockX) {
			return planeHeight + SamplePlaneHeightOffset(blockX);
		}
	}
}
