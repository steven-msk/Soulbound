using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Entity;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Json;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Core {
	[BootstrappableParentOf(typeof(PlayerController))]
	public class LevelManager : MonoBehaviour, IBootstrappable {
		private static readonly Logger logger = Logger.CreateInstance();
		public const float tickRate = 0.02f;        // 50 tps
		private float tickStartTime;
		public bool isWorldLoaded { get; private set; } = false;
		public bool IsPaused { get; private set; }

		private WorldManager worldManager;
		private string world;
		private Level level;
		public Level Level => level;

		private PlayerController player;
		public PlayerController Player => player;

		public UIManager UIManager => GameObject.Find("Canvas").GetComponent<UIManager>();

		// FEATUREIMPL: settings menu
		// FEATUREIMPL: pause menu
		// Pause menu -> Settings menu

		public const string worldDump = "worldDump.json";
		public static readonly JsonSerializerSettings globalJsonSettings = new() {
			TypeNameHandling = TypeNameHandling.Auto,
			TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
			Converters = new List<JsonConverter> { 
				new Vector2JsonConverter(),
				new Vector3JsonConverter(),
				new ColorJsonConverter()
			},
		};

		public void Init(
				WorldManager worldManager,
				string world, 
				BootstrappableInstanceFactory instanceFactory,
				Func<BootstrapTreeBuilder, IEnumerable<IBootstrappable>> treeFunc
			) {
			this.worldManager = worldManager;
			this.world = world;

			Bootstrapper bootstrapper = new();
			BootstrapTreeBuilder treeBuilder = new(null, instanceFactory);
			var tree = treeFunc.Invoke(treeBuilder).ToList();
			
			DependencyContainer dependencyContainer = bootstrapper.EarlyBootstrap(tree);
			bootstrapper.Bootstrap(tree, dependencyContainer);

			this.player = dependencyContainer.Resolve<PlayerController>();
		}

		public void BootstrapWorld(WorldDump? dump, int seed, LevelGridContext gridContext) {
			UnityEngine.Random.InitState(seed);
			this.level = new Level(player, gridContext, seed);
			this.level.BootstrapWorld(dump);
			isWorldLoaded = true;
        }

		public void SpawnPlayer(SerializedEntity? serialized) {
			if (isWorldLoaded) {
				this.level.SpawnPlayer(serialized);
			}
		}

		void IBootstrappable.OnBootstrap(DependencyContainer dependencyContainer) {
            StartCoroutine(GameTickLoop());
        }

		public void OnEarlyBootstrap(DependencyContainer dependencyContainer) {
			dependencyContainer.Register<LevelManager>(this);
		}

		private void Update() {
			if (isWorldLoaded) {
				this.level.Update(Time.deltaTime);
			}
		}

		IEnumerator GameTickLoop() {
			WaitForSecondsRealtime tickDelay = new(tickRate);
			while (Application.isPlaying) {
				if (!this.IsPaused) {
					StartTick();
					// do things
					if (isWorldLoaded) {
						level.EntityManager.Tick();
					}
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
			InvocationHelper.IfElse(this.IsPaused, 
				InputHandler.PauseInputs, 
				InputHandler.UnpauseInputs
			);
		
		}

		private void OnApplicationQuit() {
			if (isWorldLoaded) {
				worldManager.SaveWorld(world, level.CreateDump());
			}
		} 

		public static LevelManager CreateInstance() {
            GameObject? levelManagerPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("levelManager");
            return GameObject.Instantiate(levelManagerPrefab)?.GetComponent<LevelManager>()
                ?? throw new ArgumentException("LevelManager prefab not found!");
        }
	}

	public record LevelGridContext(Grid grid, Tilemap tilemap) {
		public static LevelGridContext FromRuntimePrefabs() {
			var gridPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("Grid");
			var tilemapPrefab = ResourceManager.Get<GameObject, ResourceGroups.Runtime.Prefabs>("Tilemap");

			var gridObj = GameObject.Instantiate(gridPrefab);
			var tilemapObj = GameObject.Instantiate(tilemapPrefab, gridObj.transform);
			tilemapObj.transform.SetParent(gridObj.transform);

			return new LevelGridContext(
				gridObj.GetComponent<Grid>(),
				tilemapObj.GetComponent<Tilemap>()
			);
		}
	}
}