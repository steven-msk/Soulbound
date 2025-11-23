using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Common.Json;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Zenject;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Core {
	public class LevelManager : MonoBehaviour {
		private static readonly Logger logger = Logger.CreateInstance();
		public event Action<Level>? onLevelLoaded;
		public const float tickRate = 0.02f;        // 50 tps
		private float tickStartTime;
		public bool paused { get; private set; }
		private bool sessionRunning;

		private DiContainer container = null!;
		private WorldManager worldManager = null!;
		private EntityManager entityManager = null!;
		private Canvas worldCanvas = null!;
		private InputHandler inputHandler = null!;
		private PlayerInputActions inputMappings = null!;
		public string world { get; private set; } = null!;
		public Level? level { get; private set; }
		public PlayerController? player { get; private set; }

		public UIManager UIManager => GameObject.Find("Canvas").GetComponent<UIManager>();

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

		[Inject]
		public void Construct(DiContainer container) {
			this.container = container;
			this.worldManager = container.Resolve<WorldManager>();
			this.worldCanvas = container.Resolve<Canvas>();

			inputMappings = Soulbound.instance.playerInputActions;
			InputActionMap playerActionMap = inputMappings.asset.FindActionMap("Player");
			this.inputHandler = new InputHandler(playerActionMap);

			Settings.keybindMappings.AddRebindTargetMap(playerActionMap);
			Settings.keybindMappings.ProcessBindings(new Dictionary<KeyMapping, InputAction>() {
				[KeybindMappings.jump] = inputHandler.GetAction("Jump")
			});

			inputHandler.RegisterInputEvent(inputHandler.GetAction("Esc"), pausable: false, binding => {
				binding.Performed(_ => OnEscPressed());
			});
			inputMappings.Enable();
		}

		public void BootstrapWorld(string world, WorldDump? dump, int seed, LevelGridContext gridContext) {
			UnityEngine.Random.InitState(seed);
			this.world = world;
			this.level = new Level(gridContext, seed);
			this.entityManager = new EntityManager(level);

			level.BootstrapWorld(dump, this);
			entityManager.Boostrap(dump?.serializedEntities ?? new());
			sessionRunning = true;

			container.BindInstance<Level>(level).AsSingle();
			container.BindInstance<EntityManager>(entityManager).AsSingle();
			onLevelLoaded?.Invoke(level);
		}

		public void SpawnPlayer(SerializedEntity? serialized) {
			GameObject playerPrefab = ResourceManager.GetRuntimePrefab("player");
			this.player = container.InstantiatePrefabForComponent<PlayerController>(playerPrefab);
			container.BindInstance<PlayerController>(player).AsSingle();
			var playerContext = player.GetComponent<GameObjectContext>();
			playerContext.AddNormalInstaller(new PlayerInstaller(player, worldCanvas, inputHandler));
			playerContext.Run();

			SerializedEntity fallback = new(typeof(PlayerController),
				Guid.NewGuid(), player.prefabDefinitionID,
				level.GetWorldSpawnPoint(), null
			);
			entityManager.SpawnPlayer(player, serialized ?? fallback);
		}

		private void Update() {
			if (player?.isSpawned ?? false) {
				level.UpdateChunks(player.position);
			}
			entityManager?.Update(Time.deltaTime);
		}

		IEnumerator GameTickLoop() {
			WaitForSecondsRealtime tickDelay = new(tickRate);
			while (sessionRunning) {
				if (!this.paused) {
					StartTick();
					// do things
					entityManager.Tick();
					// TODO: implement proper ticking system
					EndTick();
				}
				yield return tickDelay;
			}
		}

		public void StopSession() {
			sessionRunning = false;
			inputHandler.FlushCallbacks();
			inputMappings.Dispose();
			container.FlushBindings();
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

		public void OnChunkLoaded(WorldChunk chunk) {
			entityManager.OnChunkLoaded(chunk);
		}

		public void OnChunkUnloaded(WorldChunk chunk) {
			entityManager.OnChunkUnloaded(chunk);
		}

		public void SpawnEntity(Entity entity, EntitySpawnData spawnData) {
			entityManager.SpawnEntity(entity, spawnData);
		}

		public void RemoveEntityImmediately(Entity entity, bool destroy) {
			entityManager.RemoveEntityImmediately(entity, destroy);
		}
 
		private void OnEscPressed() {
			if (UIManager.OnEscPressed()) {
				TogglePause();
			}
		}

		public void TogglePause() {
			this.paused = !this.paused;
			Time.timeScale = this.paused ? 0f : 1f;
			inputHandler.PauseInputs(this.paused);
			UIManager.SetScreen(paused ? new GamePausedScreen().GetScreen() : null);
		}

		public WorldDump CreateDump() {
			level.Dump(out var seed, out var generatedChunks, out var structurePlacements);
			return new WorldDump(
				seed,
				generatedChunks,
				player?.Serialize() ?? default,
				structurePlacements,
				entityManager.Serialize()
			);
		}

		private void OnApplicationQuit() {
			if (sessionRunning) {
				worldManager.SaveWorld(world, CreateDump());
				StopSession();
			}
			Soulbound.instance?.OnApplicationQuit();
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