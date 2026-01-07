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

		public float SampleDensity(int blockX, int blockY, float surfaceY, BiomeWeight primary, BiomeWeight? secondary) {
			CaveModulation m1 = primary.biome.SampleCave(blockX, blockY);
			CaveModulation? m2 = secondary?.biome.SampleCave(blockX, blockY);
			float blendFactor = GetBlendFactor(primary, secondary);

			float d1 = ApplyModulation(blockX, blockY, m1);
			float d2 = m2 != null ? ApplyModulation(blockX, blockY, m2.Value) : d1;
			float blended = Mathf.Lerp(d1, d2, blendFactor);

			const float solidBias = 1f;
			float surfaceMask = GetSurfaceMask(blockY, surfaceY, m1, m2, blendFactor);
			return Mathf.Lerp(blended, solidBias, surfaceMask);
		}

		public float ApplyModulation(int blockX, int blockY, CaveModulation modulation) {
			return 1f - caveNoise.Sample2D(
				blockX * modulation.frequency,
				blockY * modulation.frequency
			) * modulation.edgeSharpness - modulation.fill;
		}

		public float GetSurfaceMask(int blockY, float surfaceY, CaveModulation m1, CaveModulation? m2, float blendFactor) {
			float s1 = m1.surfaceFalloff;
			float s2 = m2?.surfaceFalloff ?? s1;
			float surfaceFalloff = Mathf.Lerp(s1, s2, blendFactor);

			float t = Mathf.InverseLerp(surfaceY - surfaceFalloff, surfaceY, blockY);
			return 1f - Mathf.SmoothStep(1f, 0f, t);
		}
 
		public bool IsCave(float density) {
			return density <= 0f;
		}

		private float GetBlendFactor(float a, float b) {
			return b / (a + b);
		}

		private float GetBlendFactor(BiomeWeight primary, BiomeWeight? secondary) {
			float w1 = primary.value;
			float w2 = secondary?.value ?? 0f;

			return GetBlendFactor(w1, w2);
		}
	}
}
