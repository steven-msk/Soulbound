using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.Generation {
	public struct BlockContext {
		public BlockPos pos;
		public float surfaceY;
		public float distanceToSurface => surfaceY - pos.y;

		public float caveDensity;
		public bool isCave;

		public bool AboveSurface() {
			return pos.y > surfaceY;
		}
	}
}
