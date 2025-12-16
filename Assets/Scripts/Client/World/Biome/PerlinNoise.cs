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
		private readonly float frequencyX, frequencyY, frequencyZ;
		private readonly float offsetX, offsetY, offsetZ;
		private readonly float amplitude;

		public PerlinNoise(int channel, int seed, float frequencyX, float frequencyY, float amplitude) 
			: this(channel, seed) {
			this.frequencyX = frequencyX;
			this.frequencyY = frequencyY;
			this.frequencyZ = 1f / Level.CHUNK_LENGTH;
			this.amplitude = amplitude;
		}

		public PerlinNoise(int channel, int seed, float frequencyX, float frequencyY, float frequencyZ, float amplitude)
			: this(channel, seed) {
			this.frequencyX = frequencyX;
			this.frequencyY = frequencyY;
			this.frequencyZ = frequencyZ;
			this.amplitude = amplitude;
		}

		public PerlinNoise(int channel, int seed, float frequency, float amplitude) 
			: this(channel, seed) {
			this.frequencyX = this.frequencyY = this.frequencyZ = frequency;
			this.amplitude = amplitude;
		}

		private PerlinNoise(int channel, int seed) {
			this.noise = new FastNoiseLite(seed);
			this.noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

			offsetX = OffsetAxis(seed, channel, 0);
			offsetY = OffsetAxis(seed, channel, 1);
			offsetZ = OffsetAxis(seed, channel, 2);
		}

		private int OffsetAxis(int seed, int channel, int axis) {
			return (int)(Offset(seed, channel * 2 + axis) % 200000u) - 100000;
		}

		static int Offset(int seed, int channel) {
			unchecked {
				uint n = (uint)(seed ^ (channel * 0x9E3779B9));
				n ^= n >> 21;
				n *= 0x85EBCA6B;
				n ^= n >> 13;
				n *= 0xC2B2AE35;
				n ^= n >> 21;
				return (int)n;
			}
		}

		public float Sample1D(float x) {
			return noise.GetNoise(x * frequencyX + offsetX, 0f) * amplitude;
		}

		public float Sample2D(float x, float y) {
			return noise.GetNoise(x * frequencyX + offsetX, 
								  y * frequencyY + offsetY) * amplitude;
		}

		public float Sample3D(float x, float y, float z) {
			return noise.GetNoise(x * frequencyX + offsetX,
								  y * frequencyY + offsetY,
								  z * frequencyZ + offsetZ) * amplitude;
		}
	}
}
