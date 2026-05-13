using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.World.LevelDomain {
	public interface IWorldSceneRoot {
		Grid grid { get; }
		Tilemap tilemap { get; }
		Canvas canvas { get; }
		UIDocument UIDocument { get; }

		public LevelGridContext GetGridContext() => new(this.grid, this.tilemap);
	}
}
