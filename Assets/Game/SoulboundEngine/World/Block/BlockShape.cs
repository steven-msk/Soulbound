using SoulboundEngine.World.Physics;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.World.Block {
	public readonly struct BlockShape {
		public static readonly BlockShape FULL = new(new[] { AABB.UNIT_SQUARE });
		public static readonly BlockShape EMPTY = new(Array.Empty<AABB>());
		public readonly IReadOnlyList<AABB> boxes;  // local space, 0..1

		public BlockShape(IReadOnlyList<AABB> boxes) {
			this.boxes = boxes;
		}

		public bool IsEmpty => this.boxes.Count == 0;
	}
}
