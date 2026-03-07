using SoulboundBackend.Client;
using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Common;
using SoulboundBackend.Common.Json;
using SoulboundBackend.Core.AssetManagement;
using SoulboundBackend.Core.Bootstrap;
using SoulboundBackend.Core.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Core {
	public class LevelManager : MonoBehaviour, IInputContext {
		public const float tickRate = 0.02f;        // 50 tps
		private float tickStartTime;
		private float frameStartTime;
		public bool paused { get; private set; } = false;
		private bool sessionRunning;

		private DiContainer container = null!;
		private WorldRenderer worldRenderer = null!;
		private Canvas canvas = null!;
		public string world { get; private set; } = null!;
		public Level level { get; private set; } = null!;
		public Player? player { get; private set; }

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

		private static readonly RectInt simulationRect = new(-128, -76, 256, 156);
		private static readonly RectInt renderRect = new(-32, -19, 65, 39);

		[Inject]
		public void Construct(DiContainer container) {
			this.container = container;
			canvas = container.Resolve<Canvas>();
			Soulbound.instance.GetUIHandler().SetCanvas(canvas);
			Soulbound.instance.GetInputManager().PushContext(this);

			//Settings.keybindMappings.AddRebindTargetMap(playerActionMap);
			//Settings.keybindMappings.ProcessBindings(new Dictionary<KeyMapping, InputAction>() {
			//	[KeybindMappings.jump] = inputHandler.GetAction("Jump")
			//});
		}

		[PROTOTYPICAL]
		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			if (inputEvent.token.Equals(InputTokens.Keyboard.ESC)) {
				OnEscPressed();
				return true;
			}
			return false;
		}

		public void BootstrapWorld(string world, WorldDump? dump, int seed, LevelGridContext gridContext) {
			UnityEngine.Random.InitState(seed);
			this.world = world;
			level = new Level(gridContext, seed);

			worldRenderer = new WorldRenderer(simulationRect, level, gridContext.tilemap);

			level.BootstrapWorld(dump, this);
			sessionRunning = true;

			container.BindInstance(level).AsSingle();

			StartCoroutine(GameFrameLoop());
			StartCoroutine(GameTickLoop());
		}

		public Player SpawnPlayer() {
			player = new Player(canvas, level.GetWorldSpawnPoint());
			level.AddEntity(player);
			return player;
		}

		IEnumerator GameFrameLoop() {
			WaitForEndOfFrame endOfFrame = new();
			while (sessionRunning) {
				if (!paused) {
					StartFrame();

					Vector2 playerPos = player != null ? player.GetPos() : level.GetWorldSpawnPoint();
					worldRenderer.RenderView(playerPos);
					level.FrameUpdate(playerPos);

					EndFrame();
				}
				yield return endOfFrame;
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
				if (!paused) {
					StartTick();

					Vector2 playerPos = player != null ? player.GetPos() : level.GetWorldSpawnPoint();
					level.Tick(GetRelativeSimulationRect(playerPos));

					EndTick();
				}
				yield return tickDelay;
			}
		}

		private RectInt GetRelativeSimulationRect(Vector2 pivot) {
			return new(
				Mathf.FloorToInt(pivot.x) + simulationRect.x,
				Mathf.FloorToInt(pivot.y) + simulationRect.y,
				simulationRect.width,
				simulationRect.height
			);
		}

		public void StopSession() {
			sessionRunning = false;
			container.FlushBindings();
		}

		private void StartTick() {
			tickStartTime = Time.realtimeSinceStartup;
		}

		private void EndTick() {
			float elapsed = Time.realtimeSinceStartup - tickStartTime;
			if (elapsed > tickRate) {
				Logger.LogWarning($"Tick lag detected! Tick took {elapsed * 1000f:F1} ms");
			}
		}

		public void RenderRequest(BlockPos blockPos, BlockState? blockState) {
			worldRenderer.RenderBlock(blockPos, blockState);
		}

		private void OnEscPressed() {
			if (!Soulbound.instance.GetUIHandler().GetScreenNavigator().PopScreen()) {
				TogglePause();
			}
		}

		public void TogglePause() {
			paused = !paused;
			Time.timeScale = paused ? 0f : 1f;
			if (!paused) {
				Soulbound.instance.GetUIHandler().GetScreenNavigator().PopScreen();
			} else {
				Soulbound.instance.GetUIHandler().SetScreen(new GamePausedScreen());
			}
		}

		public WorldDump CreateDump() {
			level.Dump(out var seed, out var generatedChunks);

			return new WorldDump(
				seed,
				generatedChunks
			);
		}
	}

	public record LevelGridContext(Grid grid, Tilemap tilemap) {
		public static LevelGridContext FromRuntimePrefabs() {
			var gridPrefab = AssetManager.Resolve<GameObject>(new AssetKey("Grid"));
			var tilemapPrefab = AssetManager.Resolve<GameObject>(new AssetKey("Tilemap"));

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
