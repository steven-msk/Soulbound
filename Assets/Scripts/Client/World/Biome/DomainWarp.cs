using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public class DomainWarp {
		private readonly FastNoiseLite warp;

		public DomainWarp(int seed, NoiseSettings settings) {
			warp = new FastNoiseLite(seed);
			settings.ApplyTo(warp);
		}
		
		public void Warp1D(ref float x) {
			float _ = 0f;
			warp.DomainWarp(ref x, ref _);
		}

		public void Warp2D(ref float x, ref float y) {
			warp.DomainWarp(ref x, ref y);	
		}

		public void Warp3D(ref float x, ref float y, ref float z) {
			warp.DomainWarp(ref x, ref y, ref z);
		}

		public void SetAmp(float amp) {
			warp.SetDomainWarpAmp(amp);
		}

		public void SetFrequency(float frequency) {
			warp.SetFrequency(frequency);
		}
	}
}
