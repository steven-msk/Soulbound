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
		private Perlin noise;
		private readonly float frequencyX;
		private readonly float frequencyY;
		private readonly float offsetX;
		private readonly float offsetY;
		private readonly float amplitude;

		public PerlinNoise(int seed, float frequencyX, float frequencyY, float amplitude) : this(seed) {
			this.frequencyX = frequencyX;
			this.frequencyY = frequencyY;
			this.amplitude = amplitude;
		}

		public PerlinNoise(int seed, float frequency, float amplitude) : this(seed) {
			this.frequencyX = this.frequencyY = frequency;
			this.amplitude = amplitude;
		}

		private PerlinNoise(int seed) {
			this.noise = new Perlin();
			this.noise.SetSeed(seed);

			System.Random random = new System.Random(seed);
			this.offsetX = random.Next(-100000, 100000);
			this.offsetY = random.Next(-100000, 100000);
		}

		public float Sample(float arg) {
			return noise.Noise(arg * frequencyX + offsetX) * amplitude;
		}

		public float Sample(float x, float y) { 
			return noise.Noise(x * frequencyX + offsetX, y * frequencyY + offsetY) * amplitude;
		}

		public float Sample(float x, float y, float z) { 
			return noise.Noise(x * frequencyX + offsetX, y * frequencyY + offsetY, z) * amplitude; 
		}

		public float Sample1D(float x) {
			return Mathf.PerlinNoise1D(x * frequencyX + offsetX) * amplitude;
		}
	}
}
