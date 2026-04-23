using SoulboundEngine.Client.World.Chunk;
using UnityEngine;

namespace SoulboundEngine.Client.World.Generation {
	public sealed class Cavemap {
		private readonly int seed;
		private readonly NoiseSampler caveNoise;
		private readonly DomainWarp warp;
		
		public Cavemap(int seed) {
			caveNoise = new NoiseSampler(0, seed, new NoiseSettings(FastNoiseLite.NoiseType.OpenSimplex2, 1.0f));
			warp = new(seed, new NoiseSettings() {
				noiseType = FastNoiseLite.NoiseType.OpenSimplex2,
				domainWarpType = FastNoiseLite.DomainWarpType.OpenSimplex2,
				frequency = 1.0f,
				domainWarpAmp = 0.0f
			});
		}

		public float SampleDensity(int blockX, int blockY, float surfaceY, BiomeWeight primary, BiomeWeight? secondary) {
			CaveModulation m1 = primary.biome.SampleCave(blockX, blockY);
			CaveModulation? m2 = secondary?.biome.SampleCave(blockX, blockY);
			float blendFactor = GetBlendFactor(primary, secondary);

			float d1 = ApplyModulation(blockX, blockY, m1);
			float d2 = m2 != null ? ApplyModulation(blockX, blockY, m2.Value) : d1;
			float blended = Mathf.Lerp(d1, d2, blendFactor);

			float s1 = m1.surfaceFalloff;
			float? s2 = m2 != null ? m2.Value.surfaceFalloff : null;

			float b1 = m1.bottomFalloff;
			float? b2 = m2 != null ? m2.Value.bottomFalloff : null;

			const float solidBias = 1f;
			float verticalMask = GetVerticalMask(blockY, surfaceY, s1, s2, b1, b2, blendFactor);
			return Mathf.Lerp(blended, solidBias, verticalMask);
		}

		public float ApplyModulation(int blockX, int blockY, CaveModulation m) {
			float x = blockX;
			float y = blockY;
			warp.SetFrequency(m.warpFrequency);
			warp.SetAmp(m.warpAmp);
			warp.Warp2D(ref x, ref y);

			float f = 0f;
			float maxAmp = 0f;
			float amplitude = m.sharpness;
			float frequency = m.frequency;

			for (int i = 0; i < m.octaves; i++) {
				float n = caveNoise.Sample2D(x * frequency, y * frequency);
				f += amplitude * (1f - Mathf.Abs(n));

				frequency *= m.lacunarity;
				maxAmp += amplitude;
				amplitude *= m.persistence;
			}

			f /= maxAmp;
			return f - m.fill;
		}

		public float GetSurfaceMask(int blockY, float surfaceY, float s1, float? s2, float blendFactor) {
			float surfaceFalloff = Mathf.Lerp(s1, s2 ?? s1, blendFactor);

			float t = Mathf.InverseLerp(surfaceY - surfaceFalloff, surfaceY, blockY);
			return 1f - Mathf.SmoothStep(1f, 0f, t);
		}

		private float GetBottomMask(int blockY, float b1, float? b2, float blendFactor) {
			float bottomFalloff = Mathf.Lerp(b1, b2 ?? b1, blendFactor);
			
			float t = Mathf.InverseLerp(WorldChunk.minY, WorldChunk.minY + bottomFalloff, blockY);
			return Mathf.SmoothStep(1f, 0f, t);
		}

		private float GetVerticalMask(int blockY, float surfaceY, float s1, float? s2, float b1, float? b2, float blendFactor) {
			return GetSurfaceMask(blockY, surfaceY, s1, s2, blendFactor)
				 + GetBottomMask(blockY, b1, b2, blendFactor);
		}

		public bool IsCave(float density) {
			return density <= 0f;
		}

		private float GetBlendFactor(float a, float b) {
			float t = b / (a + b);
			return Mathf.SmoothStep(0f, 1f, t);
			//return b / (a + b);
		}

		private float GetBlendFactor(BiomeWeight primary, BiomeWeight? secondary) {
			float w1 = primary.value;
			float w2 = secondary?.value ?? 0f;

			return GetBlendFactor(w1, w2);
		}
	}
}
