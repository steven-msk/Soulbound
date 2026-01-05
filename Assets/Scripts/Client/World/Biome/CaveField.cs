using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.World.Generation {
	public sealed class CaveField {
		public float SampleDensity(BlockPos pos, IEnumerable<BiomeWeight> weights) {

		}

		public bool IsCave(BlockPos pos, IEnumerable<BiomeWeight> weights) {
			return SampleDensity(pos, weights) < 0f;
		}

		public bool IsCave(float density) {
			return density < 0f;
		}
	}
}
