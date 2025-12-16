using Assets.Scripts.Client.World.Biome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeEditor;
using UnityEngine;

namespace SoulboundBackend.Client.World.Generation {
	public class PerlinNoise : INoise {
		private readonly FastNoiseLite noise;
		private readonly float frequencyX;
		private readonly float frequencyY;
		private readonly float offsetX;
		private readonly float offsetY;
		private readonly float amplitude;

		public PerlinNoise(int channel, int seed, float frequencyX, float frequencyY, float amplitude) 
			: this(channel, seed) {
			this.frequencyX = frequencyX;
			this.frequencyY = frequencyY;
			this.amplitude = amplitude;
		}

		public PerlinNoise(int channel, int seed, float frequency, float amplitude) 
			: this(channel, seed) {
			this.frequencyX = this.frequencyY = frequency;
			this.amplitude = amplitude;
		}

		private PerlinNoise(int channel, int seed) {
			this.noise = new FastNoiseLite(seed);
			this.noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

			offsetX = (int)(Offset(seed, channel * 2) % 200000u) - 100000;
			offsetY = (int)(Offset(seed, channel * 2 + 1) % 200000u) - 100000;
		}

		public float Sample1D(float x) {
			return noise.GetNoise(x * frequencyX + offsetX, 0f) * amplitude;
		}

		public float Sample2D(float x, float y) {
			return noise.GetNoise(x * frequencyX + offsetX, y * frequencyY + offsetY) * amplitude;
		}

		public float Sample3D(float x, float y, float z) {
			return noise.GetNoise(x * frequencyX + offsetX, y * frequencyY + offsetY, z) * amplitude;
		}

		static int Offset(int seed, int channel) {
			uint n = (uint)(seed ^ (channel * 0x9E3779B9));
			n ^= n >> 21;
			n *= 0x85EBCA6B;
			n ^= n >> 13;
			n *= 0xC2B2AE35;
			n ^= n >> 21;

			return (int)n;
		}

	}
}
