using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.BlockSystem {
	public abstract class TileEntity {
		protected readonly Level level;
		public readonly BlockPos blockPos;

		public TileEntity(Level level, BlockPos blockPos) {
			this.level = level;
			this.blockPos = blockPos;
		}

		[Obsolete]
		public virtual void Render(BlockState blockState, Tilemap tilemap) { }
	}
}
