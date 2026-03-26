using SoulboundBackend.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.LevelDomain {
	public interface IWorldSceneRoot {
		Grid grid { get; }
		Tilemap tilemap { get; }
		Canvas canvas { get; }
		[PROTOTYPICAL] AudioSource audioSource { get; }

		public LevelGridContext GetGridContext() => new(grid, tilemap);
	}
}
