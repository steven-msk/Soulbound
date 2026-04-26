using Vendor.FastNoiseLite;

namespace SoulboundEngine.Client.World.Generation {
	public class DomainWarp {
		private readonly FastNoiseLite warp;

		public DomainWarp(int seed, NoiseSettings settings) {
			this.warp = new FastNoiseLite(seed);
			settings.ApplyTo(this.warp);
		}
		
		public void Warp1D(ref float x) {
			float _ = 0f;
			this.warp.DomainWarp(ref x, ref _);
		}

		public void Warp2D(ref float x, ref float y) {
			this.warp.DomainWarp(ref x, ref y);	
		}

		public void Warp3D(ref float x, ref float y, ref float z) {
			this.warp.DomainWarp(ref x, ref y, ref z);
		}

		public void SetAmp(float amp) {
			this.warp.SetDomainWarpAmp(amp);
		}

		public void SetFrequency(float frequency) {
			this.warp.SetFrequency(frequency);
		}
	}
}
