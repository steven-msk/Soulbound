using Assets.Scripts.Client.World.Biome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.Generation {
	public class PerlinNoise : IDensityModulation {
		private readonly float frequencyX, frequencyY;
		private readonly float amplitude;

		public PerlinNoise(float frequencyX, float frequencyY, float amplitude) {
			this.frequencyX = frequencyX;
			this.frequencyY = frequencyY;
			this.amplitude = amplitude;
		}

		public PerlinNoise(float frequency, float amplitude) {
			this.frequencyX = this.frequencyY = frequency;
			this.amplitude = amplitude;
		}

		public float Apply(float density, int x, int y) {
			return density + Mathf.PerlinNoise(x * frequencyX, y * frequencyY) * amplitude;
		}
	}
}
