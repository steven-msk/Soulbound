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

		[Obsolete]
		public IBiome ResolveBiome(int blockX) {
			IBiome targetBiome = null;
			float maxDensity = float.MinValue;

			foreach (var biome in biomes) {
				float density = biome.GetDensity(blockX);
				if (density > maxDensity) {
					maxDensity = density;
					targetBiome = biome;
				}
			}

			return targetBiome;
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
				weight = Mathf.Pow(weight, blendSharpness);

				UnityEngine.Debug.Log($"weight {weight} @ bx={blockX}, biome {biome}");
				yield return new BiomeWeight(biome, weight);
			}
		}
	}
}
