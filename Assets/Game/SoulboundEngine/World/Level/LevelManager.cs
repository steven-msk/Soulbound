using SoulboundEngine.Client;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.UI.Screen;
using SoulboundEngine.World.Biome;
using SoulboundEngine.World.Chunk;
using SoulboundEngine.World.Gen;
using System;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.World.Level {
	public class LevelManager {
		public const int CHUNK_RADIUS = 2;
		public const int TERRAIN_PLANE_Y = 0;
		public static readonly RectInt simulationView = new(-128, -76, 256, 156);
		private readonly Level level;
		private readonly SoulboundClient client;
		public bool paused { get; private set; } = false;
		private bool shouldTick;
		private IScreenHandle? pauseScreenHandle;

		public LevelManager(SoulboundClient client, ISeedProvider seedProvider, ChunkStorage chunkStorage) {
			int seed = seedProvider.GetSeed();
			PlainsBiome biome1 = new(seed);
			var biome2 = new HillsBiome(seed);
			BiomeMap biomeMap = new(new IBiome[] { biome1, biome2 });
			Heightmap heightmap = new(TERRAIN_PLANE_Y);
			Cavemap cavemap = new(seed);
			this.level = new Level(seed, new NoiseLevelChunkGenerator(biomeMap, heightmap, cavemap), CHUNK_RADIUS, chunkStorage);
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
}
