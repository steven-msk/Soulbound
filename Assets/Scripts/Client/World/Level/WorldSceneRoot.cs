using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulboundBackend.Client.World.LevelDomain {
	public sealed class WorldSceneRoot : MonoBehaviour, IWorldSceneRoot {
		[SerializeField] Grid _grid;
		[SerializeField] Tilemap _tilemap;
		[SerializeField] Canvas _canvas;

		public Grid grid => _grid;
		public Tilemap tilemap => _tilemap;
		public Canvas canvas => _canvas;
	}
}
