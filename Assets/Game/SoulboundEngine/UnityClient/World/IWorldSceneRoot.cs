namespace SoulboundEngine.UnityClient.World {
	using UnityEngine;
	using UnityEngine.Tilemaps;
	using UnityEngine.UIElements;

	public interface IWorldSceneRoot {
		Grid grid { get; }
		Tilemap tilemap { get; }
		UIDocument uiDocument { get; }
	}
}
