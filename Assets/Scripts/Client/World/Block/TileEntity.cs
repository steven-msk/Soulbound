using SoulboundBackend.Client.World.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.BlockSystem {
	public abstract class TileEntity {
		public WorldChunk chunk { get; private set; }
		public BlockPos blockPos { get; private set; }
		protected readonly Dictionary<string, object> data = new();

		public TileEntity(WorldChunk chunk, BlockPos blockPos) {
			this.chunk = chunk;
			this.blockPos = blockPos;
		}

		protected T Get<T>(string key, T defaultValue = default) {
			if (data.TryGetValue(key, out var value)) {
				return (T)value;
			}
			return defaultValue;
		}

		protected void Set<T>(string key, T value) {
			data[key] = value;
		}

		public virtual void Render(BlockState blockState, Tilemap tilemap) { }
	}
}
