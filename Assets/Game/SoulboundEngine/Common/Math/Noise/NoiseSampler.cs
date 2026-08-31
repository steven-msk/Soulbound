namespace SoulboundEngine.Common.Math.Noise {
	public sealed class NoiseSampler : INoiseSampler {
		private readonly INoise noise;
		private readonly float offsetX;
		private readonly float offsetY;
		private readonly float offsetZ;

		public NoiseSampler(int channel, NoiseSettings settings) {
			this.noise = new FastNoiseLiteAdapter(settings);

			this.offsetX = this.OffsetAxis(settings.seed, channel, 0);
			this.offsetY = this.OffsetAxis(settings.seed, channel, 1);
			this.offsetZ = this.OffsetAxis(settings.seed, channel, 2);
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
			return this.noise.GetNoise(x + this.offsetX, 0f);
		}

		public float Sample2D(float x, float y) {
			return this.noise.GetNoise(x + this.offsetX, y + this.offsetY);
		}

		public float Sample3D(float x, float y, float z) {
			return this.noise.GetNoise(x + this.offsetX, y + this.offsetY, z + this.offsetZ);
		}
	}
}
