using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.UI;
using SoulboundEngine.Client.UI.Screens;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.Generation;
using SoulboundEngine.Client.World.Render;
using SoulboundEngine.Common;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Assets;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.Client.World.LevelDomain {
	/// <summary>
	/// Manages Level lifecycles
	/// </summary>
	public class LevelManager : IInputContext {
		public const float tickRate = 0.02f;        // 50 tps
		private float tickStartTime;
		private float frameStartTime;
		public bool paused { get; private set; } = false;
		private bool sessionRunning;

		private readonly WorldRenderer worldRenderer;

		// 'readonly' means no multiple dimensions
		// this is for one dimension only
		private readonly Level level;

		public const string worldDump = "worldDump.json";
		private static readonly RectInt simulationView = new(-128, -76, 256, 156);
		private static readonly RectInt renderRect = new(-32, -19, 65, 39);

		// known issue: scattered Level and LevelManager dependencies
		public LevelManager(ISeedProvider seedProvider, BlockRenderer blockRenderer, BlockModelResolver blockModelResolver) {
			worldRenderer = new WorldRenderer(simulationView, blockRenderer, blockModelResolver);
			level = new Level(worldRenderer, seedProvider.GetSeed());
		}

		[PROTOTYPICAL]
		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			if (inputEvent.token.Equals(InputTokens.Keyboard.ESC)) {
				TogglePause();
				return true;
			}
			return false;
		}

		public void StartSession() {
			level.StartSession(new Player(level));
			sessionRunning = true;

			UniTask.Post(LevelFrameLoop);
			UniTask.Post(LevelTickLoop);
		}

		private async void LevelFrameLoop() {
			while (sessionRunning) {
				try {
					if (!paused) {
						StartFrame();

						Vector2 pivotPos = level.GetPlayer()?.GetPos() ?? level.GetWorldSpawnPoint();
						level.FrameUpdate();
						worldRenderer.RenderView(pivotPos, level.GetBlockState);

						EndFrame();
					}
					await UniTask.WaitForEndOfFrame();
				} catch (Exception e) {
					Logger.LogFatal(e);
				}
			}
		}

		private async void LevelTickLoop() {
			while (sessionRunning) {
				try {
					if (!paused) {
						StartTick();

						Vector2 pivotPos = level.GetPlayer()?.GetPos() ?? level.GetWorldSpawnPoint();
						level.Tick(GetRelativeSimulationRect(pivotPos));

						EndTick();
					}
					await UniTask.WaitForSeconds(tickRate, true);
				} catch (Exception e) {
					Logger.LogFatal(e);
				}
			}
		}

		private void StartFrame() {
			frameStartTime = Time.realtimeSinceStartup;
		}

		private void EndFrame() {
			float fps = 1f / (Time.realtimeSinceStartup - frameStartTime);
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

		public void StopSession() {
			sessionRunning = false;
			level.OnSessionStop();
		}

		private RectInt GetRelativeSimulationRect(Vector2 pivot) {
			return new(
				Mathf.FloorToInt(pivot.x) + simulationView.x,
				Mathf.FloorToInt(pivot.y) + simulationView.y,
				simulationView.width,
				simulationView.height
			);
		}

		public void TogglePause() {
			paused = !paused;
			Time.timeScale = paused ? 0f : 1f;
			UIHandler uiHandler = Soulbound.instance.GetUIHandler();
			if (!paused) {
				uiHandler.GetScreenNavigator().PopScreen();
			} else {
				uiHandler.SetScreen(new GamePausedScreen());
			}
		}

		public Level GetLevel() => level;
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
