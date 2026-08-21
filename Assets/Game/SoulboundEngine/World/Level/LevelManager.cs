namespace SoulboundEngine.World.Level {
	using SoulboundEngine.Client;
	using SoulboundEngine.Client.UI.Screen;
	using SoulboundEngine.Client.World;
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.World.Biome;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Player;
	using SoulboundEngine.World.Serialization;
	using System;
	using UnityEngine;

#nullable enable

	public class LevelManager {
		public const int CHUNK_RADIUS = 2;
		public const int TERRAIN_PLANE_Y = 0;
		public static readonly RectInt simulationView = new(-128, -76, 256, 156);
		private readonly Level level;
		private readonly SoulboundClient client;
		private readonly WorldSave save;
		private readonly EntitySerializer entitySerializer;
		private bool isBootstrapped;

		public bool paused { get; private set; } = false;
		private bool shouldTick;
		private IScreenHandle? pauseScreenHandle;

		public LevelManager(SoulboundClient client, ISeedProvider seedProvider, WorldSave save, ChunkStorage chunkStorage, EntitySerializer entitySerializer) {
			int seed = seedProvider.GetSeed();
			PlainsBiome biome1 = new(seed);
			var biome2 = new HillsBiome(seed);
			BiomeMap biomeMap = new(new IBiome[] { biome1, biome2 });
			Heightmap heightmap = new(TERRAIN_PLANE_Y);
			Cavemap cavemap = new(seed);
			this.level = new Level(seed, new NoiseLevelChunkGenerator(biomeMap, heightmap, cavemap), CHUNK_RADIUS, chunkStorage);
			this.client = client;
			this.save = save;
			this.entitySerializer = entitySerializer;
		}

		public Level Bootstrap() {
			this.level.GenerateSpawn(this.save.isNew);
			this.level.DeserializeEntities(this.entitySerializer);
			this.isBootstrapped = true;
			return this.level;
		}

		public PlayerEntity StartSession() {
			if (!this.isBootstrapped) {
				throw new InvalidOperationException("Cannot start session: Level is not bootstrapped");
			}
			PlayerEntity player = new(this.client, this.level);
			// technically, player guid should match the client's guid
			// but theres no proper way of making that guid persistent
			// so fallback to unique guid per world save
			if (!this.entitySerializer.LoadPlayer(player)) {
				player.SetGuid(Guid.NewGuid());
			}
			this.level.StartSession(player);
			this.shouldTick = true;
			if (this.save.isNew) {
				player.SetPos(this.level.GetWorldSpawnPoint());
			}
			return player;
		}

		public void Tick() {
			if (!this.shouldTick || this.paused) return;

			try {
				Vec2d pivotPos = this.level.GetPlayer()?.GetPosition() ?? this.level.GetWorldSpawnPoint();
				this.level.Tick(this.GetRelativeSimulationRect(pivotPos));
			} catch (Exception e) {
				SoulboundEngine.Logger.LogFatal(e);
			}
		}

		public void StopSession() {
			this.paused = false;
			Time.timeScale = 1f;
			this.level.OnSessionStop();
			this.entitySerializer.SaveAll(this.level.GetEntities(this.level.GetPlayer(), Entity.ALL));
			this.entitySerializer.SavePlayer(this.level.GetPlayer());
		}

		private RectInt GetRelativeSimulationRect(Vec2d pivot) {
			return new(
				Maths.FloorToInt(pivot.x) + simulationView.x,
				Maths.FloorToInt(pivot.y) + simulationView.y,
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
