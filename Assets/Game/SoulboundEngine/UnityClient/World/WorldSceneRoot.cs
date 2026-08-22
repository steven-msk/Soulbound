namespace SoulboundEngine.UnityClient.World {
	using UnityEngine;
	using UnityEngine.Tilemaps;
	using UnityEngine.UIElements;

	public sealed class WorldSceneRoot : MonoBehaviour, IWorldSceneRoot {
		[field: SerializeField] public Grid grid { get; set; }
		[field: SerializeField] public Tilemap tilemap { get; set; }
		[field: SerializeField] public UIDocument uiDocument { get; set; }
	}
}
