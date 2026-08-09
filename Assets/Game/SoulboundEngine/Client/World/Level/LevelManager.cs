using SoulboundEngine.Client.UI.Screen;
using SoulboundEngine.Client.World.Generation;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.Client.World.Level {
	using PlayerEntity = Player.PlayerEntity;

	public class LevelManager {
		public const string worldDump = "worldDump.json";
		public static readonly RectInt simulationView = new(-128, -76, 256, 156);
		private readonly Level level;
		private readonly SoulboundClient client;
		public bool paused { get; private set; } = false;
		private bool shouldTick;
		private IScreenHandle? pauseScreenHandle;

		public LevelManager(SoulboundClient client, ISeedProvider seedProvider) {
			this.level = new Level(seedProvider.GetSeed());
			this.client = client;
		}

		public PlayerEntity StartSession() {
			PlayerEntity player = new(this.client, this.level);
			this.level.StartSession(player);
			this.shouldTick = true;
			return player;
		}

		public void Tick() {
			if (!this.shouldTick || this.paused) return;

			try {
				Vector2 pivotPos = this.level.GetPlayer()?.GetPosition() ?? this.level.GetWorldSpawnPoint();
				this.level.Tick(this.GetRelativeSimulationRect(pivotPos));
			} catch (Exception e) {
				Logger.LogFatal(e);
			}
		}

		public void StopSession() {
			this.paused = false;
			Time.timeScale = 1f;
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
			if (this.paused) this.UnpauseGame();
			else this.PauseGame();
		}

		public void PauseGame() {
			this.paused = true;
			Time.timeScale = 0f;
			this.pauseScreenHandle = this.client.OpenScreen(new GamePausedScreen(this.client, this));
		}

		public void UnpauseGame() {
			this.paused = false;
			Time.timeScale = 1f;
			this.client.CloseScreen(this.pauseScreenHandle);
			this.pauseScreenHandle = null;
		}

		public Level GetLevel() => this.level;
	}

	public record LevelGridContext(Grid grid, Tilemap tilemap);
}
