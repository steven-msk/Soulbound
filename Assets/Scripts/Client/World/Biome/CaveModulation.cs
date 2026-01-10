using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public struct CaveModulation {
		public float frequency;
		public float sharpness;
		public float fill;
		public float lacunarity;
		public float persistence;
		public int octaves;
		public float surfaceFalloff;
		public float bottomFalloff;
		public float warpFrequency;
		public float warpAmp;
	}
}
