using Assets.Scripts.Client.World.Biome;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.Generation {
	public sealed class Cavemap {
		private readonly int seed;
		private readonly PerlinNoise caveNoise;
		
		public Cavemap(int seed) {
			this.caveNoise = new PerlinNoise(0, seed, frequency: 1f, amplitude: 1f);
		}

		public float SampleDensity(int blockX, int blockY, BiomeWeight primary, BiomeWeight? secondary) {
			CaveModulation m1 = primary.biome.SampleCave(blockX, blockY);
			CaveModulation? m2 = secondary?.biome.SampleCave(blockX, blockY);

			float d1 = ApplyModulation(blockX, blockY, m1);
			float d2 = m2 != null ? ApplyModulation(blockX, blockY, m2.Value) : d1;

			return BlendDensities(d1, d2, primary, secondary);
		}

		float BlendDensities(float d1, float d2, BiomeWeight primary, BiomeWeight? secondary) {
			var w1 = primary.value;
			var w2 = secondary.GetValueOrDefault().value;
			float t = secondary != null ? w2 / (w1 + w2) : 0f;

			if (secondary == null || t < 0.001f) {
				return d1;
			}

			return Mathf.Lerp(d1, d2, t);
		}

		public float ApplyModulation(int blockX, int blockY, CaveModulation modulation) {
			return 1f - caveNoise.Sample2D(
				blockX * modulation.frequency,
				blockY * modulation.frequency
			) * modulation.edgeSharpness - modulation.fill;
		}
 
		public bool IsCave(float density) {
			return density <= 0f;
		}

	}
}
