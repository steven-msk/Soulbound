using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.Generation {
	public struct BlockContext {
		public BlockPos pos;
		public int surfaceY;
		public int distanceToSurface => surfaceY - pos.y;

		public float caveDensity;
		public bool isCave;

		public bool AboveSurface() {
			return pos.y > surfaceY;
		}
	}
}
