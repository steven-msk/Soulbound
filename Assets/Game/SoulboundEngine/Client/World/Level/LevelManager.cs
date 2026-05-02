using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.Players;
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
		private readonly SoulboundClient client;

		// 'readonly' means no multiple dimensions
		// this is for one dimension only
		private readonly Level level;

		public const string worldDump = "worldDump.json";
		private static readonly RectInt simulationView = new(-128, -76, 256, 156);
		private static readonly RectInt renderRect = new(-32, -19, 65, 39);

		// known issue: scattered Level and LevelManager dependencies
		public LevelManager(SoulboundClient client, ISeedProvider seedProvider, BlockRenderer blockRenderer, BlockModelResolver blockModelResolver) {
			this.worldRenderer = new WorldRenderer(simulationView, blockRenderer, blockModelResolver);
			this.level = new Level(this.worldRenderer, seedProvider.GetSeed());
			this.client = client;
		}

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			yield return InputEventListener.ConsumePerformed(InputTokens.Keyboard.ESC, _ => this.TogglePause());
		}

		public void StartSession() {
			this.level.StartSession(new Player(this.level));
			this.sessionRunning = true;

			UniTask.Post(this.LevelFrameLoop);
			UniTask.Post(this.LevelTickLoop);
		}

		private async void LevelFrameLoop() {
			while (this.sessionRunning) {
				if (!this.paused) {
					this.StartFrame();

					try {
						Vector2 pivotPos = this.level.GetPlayer()?.GetPosition() ?? this.level.GetWorldSpawnPoint();
						this.level.FrameUpdate();
						this.worldRenderer.RenderView(pivotPos, this.level.GetBlockState);
					} catch (Exception e) {
						Logger.LogFatal(e);
					}

					this.EndFrame();
				}
				await UniTask.NextFrame();
			}
		}

		private async void LevelTickLoop() {
			while (this.sessionRunning) {
				if (!this.paused) {
					this.StartTick();

					try {
						Vector2 pivotPos = this.level.GetPlayer()?.GetPosition() ?? this.level.GetWorldSpawnPoint();
						this.level.Tick(this.GetRelativeSimulationRect(pivotPos));
					} catch (Exception e) {
						Logger.LogFatal(e);
					}

					this.EndTick();
				}
				await UniTask.WaitForSeconds(tickRate, true);
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

		private void TogglePause() {
			if (this.paused) this.UnpauseGame();
			else this.PauseGame();
		}

		public void PauseGame() {
			this.paused = true;
			Time.timeScale = 0f;
			this.client.UIHandler.GetScreenNavigator().PushScreen(new GamePausedScreen(this.client, this));
			this.client.InputManager.RemoveHandler(this.level.GetPlayer());
		}

		public void UnpauseGame() {
			this.paused = false;
			Time.timeScale = 1f;
			this.client.UIHandler.GetScreenNavigator().PopScreen();
			this.client.InputManager.AddHandler(this.level.GetPlayer());
		}

		public Level GetLevel() => this.level;
	}

	public record LevelGridContext(Grid grid, Tilemap tilemap);
}
