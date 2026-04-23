using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundEngine.Client.World.LevelDomain {
	public interface IWorldSceneRoot {
		Grid grid { get; }
		Tilemap tilemap { get; }
		Canvas canvas { get; }

		public LevelGridContext GetGridContext() => new(grid, tilemap);
	}
}
