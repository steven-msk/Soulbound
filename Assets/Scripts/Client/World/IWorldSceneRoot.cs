using SoulboundBackend.Core;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World {
	public interface IWorldSceneRoot {
		Grid grid { get; }
		Tilemap tilemap { get; }
		Canvas canvas { get; }

		public LevelGridContext GetGridContext() => new(grid, tilemap);
	}
}
