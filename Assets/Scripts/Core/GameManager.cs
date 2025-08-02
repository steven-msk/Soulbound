using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	// POTENTIAL FEATUREIMPL (unlikely, but possible): make Level class NOT a singleton.
	// In this case, it represents the current dimension or world the player is in (e.g., overworld, nether, end).
	// The GameManager holds a reference to the active Level instance.
	// If implementing multiple dimensions, switch the Level reference as the player moves between them.

    private Level level;
	public Level Level => level;

	[SerializeField] private Tilemap worldTilemap;

	public bool IsPaused { get; private set; }

	public PlayerController Player => GameObject.Find("johnny").GetComponent<PlayerController>();

	public UIController UI => GameObject.Find("Canvas").GetComponent<UIController>();
	 
	// FEATUREIMPL: settings menu
	// FEATUREIMPL: pause menu
	// Pause menu -> Settings menu

	private void Awake() {
		instance = this;

		ReloadRegistries();

		int seed = 745632;           // UnityEngine.Random.Range(int.MinValue, int.MaxValue)
		this.level = new Level(Player, worldTilemap, GameObject.Find("Grid").GetComponent<Grid>(), seed, renderDistance: 2);
		this.level.EarlyGenerateChunks(Player.position);
		UnityEngine.Random.InitState(seed);
		LogUtil.LogAwake(this);
	}

	private void OnValidate() => ReloadRegistries();

	private void Start() {
		StartCoroutine(GameTickLoop());
	}

	private void ReloadRegistries() {
		Registry.Reset();
		Registry.RegisterAll<TMP_FontAsset>("Registry/Fonts");
		Registry.RegisterAll<Item>("Registry/Items");
		Registry.RegisterAll<GameObject>("Registry/Prefabs");
		Registry.RegisterAll<Tile>("Registry/Tiles");
		Registry.RegisterAll<RuleTile>("Registry/Tiles");
		Registry.RegisterAll<Block>("Registry/Blocks");
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
		AudioListener.pause = this.IsPaused;		// FEATUREIMPL: sound effects and music
		InvocationHelper.IfElse(this.IsPaused, InputHandler.PauseInputs, InputHandler.UnpauseInputs);
		
	}

	private void OnApplicationQuit() {
		EventBus<GameEvent>.Clear();
		EventBus<SystemEvent>.Clear();
	}
}