namespace SoulboundEngine.Client.World {
	using UnityEngine;
	using UnityEngine.Tilemaps;
	using UnityEngine.UIElements;

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
