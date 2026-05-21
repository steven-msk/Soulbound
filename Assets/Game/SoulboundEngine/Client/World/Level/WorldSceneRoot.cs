using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.World.LevelDomain {
	public sealed class WorldSceneRoot : MonoBehaviour, IWorldSceneRoot {
		[SerializeField] Grid _grid;
		[SerializeField] Tilemap _tilemap;
		[SerializeField] Canvas _canvas;
		[SerializeField] UIDocument uiDocument;

		public Grid grid => this._grid;
		public Tilemap tilemap => this._tilemap;
		public Canvas canvas => this._canvas;
		public UIDocument UIDocument => this.uiDocument;
	}
}
