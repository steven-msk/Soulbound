using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	private Level level;
	public Level Level => level;

	[SerializeField] private Tilemap worldTilemap;

	public static bool IsPaused { get; private set; }

	public static PlayerController GetPlayerInstance() => GameObject.Find("johnny").GetComponent<PlayerController>();

	public static UIController GetUI() => GameObject.Find("Canvas").GetComponent<UIController>();

	private void Awake() {
		instance = this;

		Registry.RegisterAll<TMP_FontAsset>("Registry/Fonts");
		Registry.RegisterAll<Item>("Registry/Items");
		Registry.RegisterAll<GameObject>("Registry/Prefabs");
		Registry.RegisterAll<Tile>("Registry/Tiles");
		Registry.RegisterAll<RuleTile>("Registry/Tiles");

		this.level = new Level(GetPlayerInstance(), worldTilemap);
	}

	private void OnApplicationQuit() {
		EventBus<GameEvent>.Clear();
		EventBus<SystemEvent>.Clear();
		Registry.Reset();
	}
}