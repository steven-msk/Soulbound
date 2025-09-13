using SoulboundBackend.Client.ItemSystem;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	public abstract partial class Block {
		public abstract string name { get; }
		public abstract TileBase tileReference { get; }
		public abstract BlockItem? itemReference { get; }
		public abstract BlockState defaultState { get; }

		public virtual BlockState CreateState(Dictionary<string, object> properties) {
			return defaultState;
		}

		public override string ToString() {
			return name;
		}
	}
}