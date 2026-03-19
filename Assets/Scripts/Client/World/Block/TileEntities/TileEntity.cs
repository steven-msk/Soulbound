using SoulboundBackend.Client.World.BlockSystem.States;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.LevelDomain;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.BlockSystem.TileEntities {
	public abstract class TileEntity {
		protected readonly Level level;
		public readonly BlockPos blockPos;

		public TileEntity(Level level, BlockPos blockPos) {
			this.level = level;
			this.blockPos = blockPos;
		}

		[Obsolete]
		public virtual void Render(BlockState blockState, Tilemap tilemap) { }

		public virtual void OnDispose() { }
	}
}
