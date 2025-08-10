using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

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

	private Dictionary<Type, List<Object>> registriesByType = new();

	// FEATUREIMPL: settings menu
	// FEATUREIMPL: pause menu
	// Pause menu -> Settings menu

	private void Awake() {
		instance = this;

		ResourceGroups.Bootstrap();
		ReloadRegistries();

		int seed = 745632;           // UnityEngine.Random.Range(int.MinValue, int.MaxValue)
		this.level = new Level(Player, worldTilemap, GameObject.Find("Grid").GetComponent<Grid>(), seed, renderDistance: 2);
		this.level.BootstrapWorld(Player.position);
		UnityEngine.Random.InitState(seed);
		LogUtil.LogAwake(this);
	}

	private void OnValidate() => ReloadRegistries();

	private void Start() {
		StartCoroutine(GameTickLoop());
	}

	private void ReloadRegistries() {
		if (registriesByType.Count > 0) {
			Debug.LogWarning("Registries already loaded. Skipping reload.");
            return;
        }

        AssetRegistry.Reset();
		registriesByType.Clear();
		RegisterByType<TMP_FontAsset>(AssetRegistry.RegisterAll<TMP_FontAsset>("Registry/Fonts"));
		//RegisterByType<Item>(AssetRegistry.RegisterAll<Item>("Registry/Items"));
		//RegisterBySubclassedType<Item, BlockItem>(registriesByType[typeof(Item)].Cast<Item>().ToList());
		RegisterByType<GameObject>(AssetRegistry.RegisterAll<GameObject>("Registry/Prefabs"));
		RegisterByType<Tile>(AssetRegistry.RegisterAll<Tile>("Registry/Tiles"));
		RegisterByType<RuleTile>(AssetRegistry.RegisterAll<RuleTile>("Registry/Tiles"));
		RegisterByType<Sprite>(AssetRegistry.RegisterAll<Sprite>("Registry/Sprites"));
		//RegisterByType<Block>(AssetRegistry.RegisterAll<Block>("Registry/Blocks"));
    }

	public List<T> GetAll<T>() where T : UnityEngine.Object {
		if (registriesByType.TryGetValue(typeof(T), out List<Object> resources)) {
			return resources.Cast<T>().ToList();
		}
		Debug.LogError($"No resources of type '{typeof(T).Name}' found in registries.");
		return new List<T>();
    }

    private void RegisterByType<T>(List<T> registeredResources, bool logRegistration = true) where T : UnityEngine.Object {
		registriesByType[typeof(T)] = registeredResources.Select(resource => (Object)resource).ToList();
		logRegistration.If(() => Debug.Log($"Registered {registeredResources.Count} resources of type '{typeof(T).Name}'"));
    }

	private void RegisterBySubclassedType<T, TSub>(List<T> registered) where T : UnityEngine.Object where TSub : T {
		List<TSub> subclassed = registered.Where(resource => resource is TSub).Cast<TSub>().ToList();
		RegisterByType<TSub>(subclassed, logRegistration: false);
		Debug.Log($"Registered {subclassed.Count} resources of type '{typeof(TSub).Name}' as subclassed type of '{typeof(T).Name}'");
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