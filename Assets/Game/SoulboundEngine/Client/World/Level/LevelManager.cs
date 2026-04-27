using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.UI;
using SoulboundEngine.Client.UI.Screens;
using SoulboundEngine.Client.World.BlockSystem.Render;
using SoulboundEngine.Client.World.Generation;
using SoulboundEngine.Client.World.Render;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.Client.World.LevelDomain {
	/// <summary>
	/// Manages Level lifecycles
	/// </summary>
	public class LevelManager : IInputEventHandler {
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
			this.worldRenderer = new WorldRenderer(simulationView, blockRenderer, blockModelResolver);
			this.level = new Level(this.worldRenderer, seedProvider.GetSeed());
		}

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			yield return new(InputTokens.Keyboard.ESC, InputEvent.Phase.Any, _ => {
				this.TogglePause();
				return true;
			});
		}

		public void StartSession() {
			this.level.StartSession(new Player(this.level));
			this.sessionRunning = true;

			UniTask.Post(this.LevelFrameLoop);
			UniTask.Post(this.LevelTickLoop);
		}

		private async void LevelFrameLoop() {
			while (this.sessionRunning) {
				try {
					if (!this.paused) {
						this.StartFrame();

						Vector2 pivotPos = this.level.GetPlayer()?.GetPos() ?? this.level.GetWorldSpawnPoint();
						this.level.FrameUpdate();
						this.worldRenderer.RenderView(pivotPos, this.level.GetBlockState);

						this.EndFrame();
					}
					await UniTask.WaitForEndOfFrame();
				} catch (Exception e) {
					Logger.LogFatal(e);
				}
			}
		}

		private async void LevelTickLoop() {
			while (this.sessionRunning) {
				try {
					if (!this.paused) {
						this.StartTick();

						Vector2 pivotPos = this.level.GetPlayer()?.GetPos() ?? this.level.GetWorldSpawnPoint();
						this.level.Tick(this.GetRelativeSimulationRect(pivotPos));

						this.EndTick();
					}
					await UniTask.WaitForSeconds(tickRate, true);
				} catch (Exception e) {
					Logger.LogFatal(e);
				}
			}
		}

		private void StartFrame() {
			this.frameStartTime = Time.realtimeSinceStartup;
		}

		private void EndFrame() {
			float fps = 1f / (Time.realtimeSinceStartup - this.frameStartTime);
		}

		private void StartTick() {
			this.tickStartTime = Time.realtimeSinceStartup;
		}

		private void EndTick() {
			float elapsed = Time.realtimeSinceStartup - this.tickStartTime;
			if (elapsed > tickRate) {
				Logger.LogWarning($"Tick lag detected! Tick took {elapsed * 1000f:F1} ms");
			}
		}

		public void StopSession() {
			this.sessionRunning = false;
			this.level.OnSessionStop();
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
			this.paused = !this.paused;
			Time.timeScale = this.paused ? 0f : 1f;
			UIHandler uiHandler = SoulboundClient.Instance.UIHandler;
			if (!this.paused) {
				uiHandler.GetScreenNavigator().PopScreen();
			} else {
				uiHandler.SetScreen(new GamePausedScreen());
			}
		}

		public Level GetLevel() => this.level;
	}

	public record LevelGridContext(Grid grid, Tilemap tilemap);
}
