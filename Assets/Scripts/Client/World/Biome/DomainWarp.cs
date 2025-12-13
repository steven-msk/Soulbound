using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public class DomainWarp {
		private readonly FastNoiseLite warp;

		public DomainWarp(int seed, float frequency, float strength = 1.0f) {
			this.warp = new FastNoiseLite(seed);
			warp.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
			warp.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
			warp.SetDomainWarpAmp(strength);
			warp.SetFrequency(frequency);
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
	}
}
