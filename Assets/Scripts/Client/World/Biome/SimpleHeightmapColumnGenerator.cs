using Assets.Scripts.Client.World.Biome;
using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace SoulboundBackend.Client.World.Generation {
	public class SimpleHeightmapColumnGenerator : IDensityGenerator {
		private readonly int height;
		private readonly IBiome biome;
		private IEnumerable<IDensityModulation> modulations;

		public SimpleHeightmapColumnGenerator(IBiome biome, int height, params IDensityModulation[] densityModulations) {
			this.height = height;
			this.biome = biome;
			this.modulations = densityModulations;
		}

		public float SampleDensity(int x, int y) {
			float density = height - y;
			foreach (var modulation in modulations) {
				density = modulation.Apply(density, x, y);
			}
			return density;
		}

		public BlockState ResolveBlock(int x, int y) {
			return biome.ResolveBlock(SampleDensity(x, y), x, y);
		}
	}
}
