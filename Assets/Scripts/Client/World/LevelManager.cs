using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.SettingSystem;
using SoulboundBackend.Client.UI;
using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Common.Json;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
		private float frameStartTime;
		public bool paused { get; private set; }
		private bool sessionRunning;

		private DiContainer container = null!;
		private WorldManager worldManager = null!;
		private WorldRenderer worldRenderer = null!;
		public EntityManager entityManager { get; private set; } = null!;
		private Canvas worldCanvas = null!;
		private InputHandler inputHandler = null!;
		private PlayerInputActions inputMappings = null!;
		public string world { get; private set; } = null!;
		public Level level { get; private set; } = null!;
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

			inputMappings = container.Resolve<PlayerInputActions>();
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
			this.entityManager = new EntityManager(level, new GameObject("Updater").AddComponent<UpdateManager>());

			RectInt renderRect = new(-128, -76, 256, 156);      // simulation distance
			//RectInt renderRect = new(-32, -19, 65, 39);		// default view distance
			this.worldRenderer = new WorldRenderer(renderRect, level, gridContext.tilemap);

			level.BootstrapWorld(dump, this);
			entityManager.Deserialize(dump?.serializedEntities ?? new());
			sessionRunning = true;

			container.BindInstance<Level>(level).AsSingle();
			container.BindInstance<EntityManager>(entityManager).AsSingle();

			StartCoroutine(GameFrameLoop());
			StartCoroutine(GameTickLoop());
			onLevelLoaded?.Invoke(level);
		}

		public void SpawnPlayer(SerializedEntity? serialized) {
			if (entityManager.GetEntityByID(serialized.GetValueOrDefault().id, out var playerEntity)) {
				entityManager.RemoveEntity(playerEntity);
			}
			this.player = container.InstantiatePrefabForComponent<PlayerController>(
				ResourceManager.GetRuntimePrefab("player")
			);
			container.BindInstance<PlayerController>(this.player).AsSingle();
			var playerContext = this.player.GetComponent<GameObjectContext>();
			playerContext.AddNormalInstaller(new PlayerInstaller(this.player, worldCanvas, inputHandler));
			playerContext.Run();

			if (serialized.HasValue) {
				this.player.Deserialize(serialized.Value);
			}
			entityManager.Spawn<PlayerController, PlayerSpawnData>(this.player, new PlayerSpawnData() {
				position = serialized?.lastPosition ?? level.GetWorldSpawnPoint()
			});
		}

		private void Update() {
			level?.UpdateChunks(player?.position ?? new Vector2(0f, 0f));
			entityManager?.Update(Time.deltaTime);
		}

		IEnumerator GameFrameLoop() {
			WaitForEndOfFrame endOfFrame = new();
			while (sessionRunning) {
				if (!this.paused) {
					StartFrame();

					worldRenderer.RenderView(player?.position ?? level.GetWorldSpawnPoint());

					yield return endOfFrame;

					EndFrame();
				}
			}
		}

		private void StartFrame() {
			frameStartTime = Time.realtimeSinceStartup;
		}

		private void EndFrame() {
			float fps = 1f / (Time.realtimeSinceStartup - frameStartTime);
		}

		IEnumerator GameTickLoop() {
			WaitForSecondsRealtime tickDelay = new(tickRate);
			while (sessionRunning) {
				if (!this.paused) {
					StartTick();
					// do things
					entityManager.Tick();
					level.Tick();
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
				logger.LogWarning($"Tick lag detected! Tick took {elapsed * 1000f:F1} ms");
			}
		}

		public void RenderRequest(BlockPos blockPos, BlockState? blockState) {
			worldRenderer.RenderBlock(blockPos, blockState);
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
			var serializedPlayer = player?.Serialize() ?? default;
			var serializedEntities = entityManager.Serialize();
			serializedEntities.Remove(serializedPlayer.id);

			return new WorldDump(
				seed,
				generatedChunks,
				serializedPlayer,
				structurePlacements,
				serializedEntities
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
			GameObject? levelManagerPrefab = ResourceManager.GetRuntimePrefab("levelManager");
			return GameObject.Instantiate(levelManagerPrefab)?.GetComponent<LevelManager>()
				?? throw new ArgumentException("LevelManager prefab not found!");
		}
	}

	public record LevelGridContext(Grid grid, Tilemap tilemap) {
		public static LevelGridContext FromRuntimePrefabs() {
			var gridPrefab = ResourceManager.GetRuntimePrefab("Grid");
			var tilemapPrefab = ResourceManager.GetRuntimePrefab("Tilemap");

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