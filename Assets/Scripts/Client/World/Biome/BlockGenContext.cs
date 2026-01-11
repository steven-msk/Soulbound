using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.World.Generation {
	public struct BlockGenContext {
		public BlockPos pos;
		public int surfaceY;
		public float caveDensity;
		public bool isCave;

		public int distanceToSurface => Mathf.Abs(surfaceY - pos.y);
		public int signedDistanceToSurface => surfaceY - pos.y;

		public bool AboveSurface() {
			return pos.y > surfaceY;
		}
	}
}
