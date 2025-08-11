using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

public class GameManager : MonoBehaviour {
	private static readonly Logger logger = Logger.CreateInstance();
	public static GameManager instance;
	public const float tickRate = 0.02f;        // 50 tps
	private float tickStartTime;

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

		ResourceGroups.Bootstrap();

		int seed = 745632;           // UnityEngine.Random.Range(int.MinValue, int.MaxValue)
		this.level = new Level(Player, worldTilemap, GameObject.Find("Grid").GetComponent<Grid>(), seed, renderDistance: 2);
		this.level.BootstrapWorld(Player.position);
		UnityEngine.Random.InitState(seed);
	}

	private void OnValidate() => ResourceGroups.Bootstrap();

	private void Start() {
		StartCoroutine(GameTickLoop());
	}

	private void Update() {
		this.level.Update(Time.deltaTime);
	}

	IEnumerator GameTickLoop() {
		WaitForSecondsRealtime tickDelay = new(tickRate);
		while (Application.isPlaying) {
			if (!this.IsPaused) {
				StartTick();
				// do things
				level.EntityManager.Tick();
				// TODO: implement proper ticking system
				EndTick();
			}
			yield return tickDelay;
		}
	}

	private void StartTick() {
		tickStartTime = Time.realtimeSinceStartup;
	}

	private void EndTick() {
		float elapsed = Time.realtimeSinceStartup - tickStartTime;
		if (elapsed > tickRate) {
			logger.LogWarning(null, $"Tick lag detected! Tick took {elapsed * 1000f:F1} ms");
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