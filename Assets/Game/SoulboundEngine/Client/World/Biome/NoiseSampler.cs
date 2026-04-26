using UnityEngine;
using Vendor.FastNoiseLite;

namespace SoulboundEngine.Client.World.Generation {
	public sealed class NoiseSampler : INoise {
		private readonly FastNoiseLite noise;
		private readonly Vector3 offset;

		public NoiseSampler(int channel, int seed, NoiseSettings settings) {
			this.noise = new FastNoiseLite(seed);
			settings.ApplyTo(this.noise);

			float offsetX = this.OffsetAxis(seed, channel, 0);
			float offsetY = this.OffsetAxis(seed, channel, 1);
			float offsetZ = this.OffsetAxis(seed, channel, 2);
			this.offset = new Vector3(offsetX, offsetY, offsetZ);
		}

		private int OffsetAxis(int seed, int channel, int axis) {
			return (int)(OffsetChannel(seed, channel * 2 + axis) % 200000u) - 100000;
		}

		static int OffsetChannel(int seed, int channel) {
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
			return this.noise.GetNoise(x + this.offset.x, 0f);
		}

		public float Sample2D(float x, float y) {
			return this.noise.GetNoise(x + this.offset.x, y + this.offset.y);
		}

		public float Sample3D(float x, float y, float z) {
			return this.noise.GetNoise(x + this.offset.x, y + this.offset.y, z + this.offset.z);
		}
	}
}
