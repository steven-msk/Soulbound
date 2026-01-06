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
			CaveModulation modulation = BlendModulations(blockX, blockY, primary, secondary);
			float noise = ApplyModulation(blockX, blockY, modulation);
			return 1f - noise - modulation.fill;
		}

		CaveModulation BlendModulations(int blockX, int blockY, BiomeWeight primary, BiomeWeight? secondary) {
			var w1 = primary.value;
			var w2 = secondary.GetValueOrDefault().value;
			float t = secondary != null ? w2 / (w1 + w2) : 0f;

			var a = primary.biome.SampleCave(blockX, blockY);
			if (secondary == null || t < 0.001f) {
				return a;
			}
			var b = secondary.Value.biome.SampleCave(blockX, blockY);

			return new CaveModulation {
				frequency = Mathf.Lerp(a.frequency, b.frequency, t),
				edgeSharpness = Mathf.Lerp(a.edgeSharpness, b.edgeSharpness, t),
				fill = Mathf.Lerp(a.fill, b.fill, t),
			};
		}

		public float ApplyModulation(int blockX, int blockY, CaveModulation modulation) {
			return caveNoise.Sample2D(
				blockX * modulation.frequency,
				blockY * modulation.frequency
			) * modulation.edgeSharpness;
		}
 
		public bool IsCave(float density) {
			return density <= 0f;
		}
	}
}
