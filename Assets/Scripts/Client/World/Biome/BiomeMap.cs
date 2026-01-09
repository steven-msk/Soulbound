using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.Generation {
	public sealed class BiomeMap {
		const float blendSharpness = 3f;

		private readonly IEnumerable<IBiome> biomes;

		public BiomeMap(IEnumerable<IBiome> biomes) {
			this.biomes = biomes;
		}

		public IEnumerable<BiomeWeight> ResolveWeights(int blockX) {
			List<(IBiome biome, float value)> densities = new();
			float maxDensity = 0f;

			foreach (var biome in biomes) {
				float density = biome.GetDensity(blockX);
				maxDensity = Mathf.Max(maxDensity, density);

				if (density > 0) {
					densities.Add((biome, density));
				}
			}
			if (maxDensity <= 0f) {
				yield break;
			}

			foreach (var (biome, density) in densities) {
				float weight = density / maxDensity;
				//weight = Mathf.SmoothStep(0f, 1f, weight);
				weight = Mathf.Pow(weight, blendSharpness);

				yield return new BiomeWeight(biome, weight);
			}
		}

		public void ResolvePrimaryBiomes(IEnumerable<BiomeWeight> weights, out BiomeWeight primary, out BiomeWeight? secondary) {
			List<BiomeWeight> orderedWeights = weights
				.OrderByDescending(w => w.value)
				.ToList();
			if (weights.Count() == 0) {
				primary = default;
				secondary = null;
			}

			primary = orderedWeights[0];
			secondary = orderedWeights.Count > 1 ? orderedWeights[1] : null;
		}
	}
}
