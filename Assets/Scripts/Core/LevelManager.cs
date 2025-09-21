using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Client.World;
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
		public static LevelManager instance;
		public const float tickRate = 0.02f;        // 50 tps
		private float tickStartTime;
		public bool IsPaused { get; private set; }

		// POTENTIAL FEATUREIMPL (unlikely, but possible): make Level class NOT a singleton.
		// In this case, it represents the current dimension or world the player is in (e.g., overworld, nether, end).
		// The GameManager holds a reference to the active Level instance.
		// If implementing multiple dimensions, switch the Level reference as the player moves between them.

		private Level level;
		public Level Level => level;

		private PlayerController player;
		public PlayerController Player => player;

		public UIManager UIManager => GameObject.Find("Canvas").GetComponent<UIManager>();

		[SerializeField] private Tilemap worldTilemap;

		// FEATUREIMPL: settings menu
		// FEATUREIMPL: pause menu
		// Pause menu -> Settings menu

		public const string worldDump = "worldDump.json";
		public static readonly JsonSerializerSettings globalJsonSettings = new() {
			TypeNameHandling = TypeNameHandling.Auto,
			TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
			Converters = new List<JsonConverter> { 
				new Vector2JsonConverter(), new Vector3JsonConverter(), new ColorJsonConverter()
			},
		};

		public void Init(List<IBootstrappable> bootstrappables, Func<BootstrapTreeBuilder, IEnumerable<IBootstrappable>> treeFunc) {
            instance = this;

            Bootstrapper bootstrapper = new();
			BootstrapTreeBuilder treeBuilder = new(bootstrappables);
			var tree = treeFunc.Invoke(treeBuilder).ToList();
			
			DependencyContainer dependencyContainer = bootstrapper.EarlyBootstrap(tree);
			bootstrapper.Bootstrap(tree, dependencyContainer);

			this.player = dependencyContainer.Resolve<PlayerController>();
            int seed = 745632;           // UnityEngine.Random.Range(int.MinValue, int.MaxValue)
            WorldDump? worldDump;
            try {
                worldDump = JsonConvert.DeserializeObject<WorldDump>(File.ReadAllText(Level.worldDumpFile), globalJsonSettings);
                seed = worldDump?.seed ?? seed;
            } catch (FileNotFoundException) {
                logger.LogError(null, "Cannot find world dump file");
                worldDump = null;
            }
            UnityEngine.Random.InitState(seed);
            this.level = new Level(player, worldTilemap, GameObject.Find("Grid").GetComponent<Grid>(), seed, renderDistance: 2);
            this.level.BootstrapWorld(worldDump);
        }

        public void OnBootstrap(DependencyContainer dependencyContainer) {
        }

        public void OnEarlyBootstrap(DependencyContainer dependencyContainer) {
			dependencyContainer.Register<LevelManager>(this);
        }

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
			level.Save();
		}
    }
}