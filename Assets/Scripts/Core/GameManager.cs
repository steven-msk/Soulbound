using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	private Level level;
	public Level Level => level;

	[SerializeField] private Tilemap worldTilemap;

	public bool IsPaused { get; private set; }

	public PlayerController Player => GameObject.Find("johnny").GetComponent<PlayerController>();

	public UIController UI => GameObject.Find("Canvas").GetComponent<UIController>();

	// FEATUREIMPL: settings menu
	// FEATUREIMPL: pause menu

	private void Awake() {
		instance = this;

		Registry.RegisterAll<TMP_FontAsset>("Registry/Fonts");
		Registry.RegisterAll<Item>("Registry/Items");
		Registry.RegisterAll<GameObject>("Registry/Prefabs");
		Registry.RegisterAll<Tile>("Registry/Tiles");
		Registry.RegisterAll<RuleTile>("Registry/Tiles");

		this.level = new Level(Player, worldTilemap, GameObject.Find("Grid").GetComponent<Grid>(), renderDistance: 2);
		this.level.EarlyGenerateChunks(Player.position);
		LogUtil.LogAwake(this);
	}

	private void Start() {
		StartCoroutine(GameTickLoop());
	}

	IEnumerator GameTickLoop() {
		while (Application.isPlaying) {
			if (!this.IsPaused) {
				// TODO: implement ticking system
			}
			yield return new WaitForSecondsRealtime(0.1f);		// TODO: decide on a tick rate
		}
	}

	public void TogglePauseGame() {
		this.IsPaused = !this.IsPaused;
		Time.timeScale = this.IsPaused ? 0f : 1f;
		AudioListener.pause = this.IsPaused;
		InvocationHelper.IfElse(this.IsPaused, InputHandler.PauseInputs, InputHandler.UnpauseInputs);
		
	}

	private void OnApplicationQuit() {
		EventBus<GameEvent>.Clear();
		EventBus<SystemEvent>.Clear();
		Registry.Reset();
	}
}