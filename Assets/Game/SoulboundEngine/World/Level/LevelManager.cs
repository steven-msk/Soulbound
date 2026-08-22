namespace SoulboundEngine.World.Level {
	using SoulboundEngine.Common.Math;
	using SoulboundEngine.Recipe;
	using SoulboundEngine.World.Biome;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Gen;
	using SoulboundEngine.World.Physics;
	using SoulboundEngine.World.Player;
	using SoulboundEngine.World.Serialization;
	using System;

#nullable enable

	public class LevelManager {
		public const int CHUNK_RADIUS = 2;
		public const int TERRAIN_PLANE_Y = 0;
		private static readonly Vec2i SIMULATION_VIEW_SIZE = new(256, 156);
		private readonly Level level;
		private readonly WorldSave save;
		private readonly EntitySerializer entitySerializer;
		private bool isBootstrapped;

		public bool paused { get; private set; } = false;
		private bool shouldTick;

		public LevelManager(ISeedProvider seedProvider, WorldSave save, RecipeManager recipeManager, ChunkStorage chunkStorage, EntitySerializer entitySerializer) {
			int seed = seedProvider.GetSeed();
			PlainsBiome biome1 = new(seed);
			HillsBiome biome2 = new(seed);
			BiomeMap biomeMap = new(new IBiome[] { biome1, biome2 });
			Heightmap heightmap = new(TERRAIN_PLANE_Y);
			Cavemap cavemap = new(seed);
			this.level = new Level(seed, recipeManager, new NoiseLevelChunkGenerator(biomeMap, heightmap, cavemap), CHUNK_RADIUS, chunkStorage);
			this.save = save;
			this.entitySerializer = entitySerializer;
		}

		public Level Bootstrap() {
			this.level.GenerateSpawn(this.save.isNew);
			this.level.DeserializeEntities(this.entitySerializer);
			this.isBootstrapped = true;
			return this.level;
		}

		public PlayerEntity StartSession(Func<Level, PlayerEntity> playerFactory) {
			if (!this.isBootstrapped) {
				throw new InvalidOperationException("Cannot start session: Level is not bootstrapped");
			}
			PlayerEntity player = playerFactory(this.level);
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
			this.level.OnSessionStop();
			this.entitySerializer.SaveAll(this.level.GetEntities(this.level.GetPlayer(), Entity.ALL));
			this.entitySerializer.SavePlayer(this.level.GetPlayer());
		}

		private AABB GetRelativeSimulationRect(Vec2d pivot) {
			return AABB.OfSize(pivot.Floor(), SIMULATION_VIEW_SIZE.x, SIMULATION_VIEW_SIZE.y);
		}

		public bool TogglePause() {
			if (this.paused) this.UnpauseGame();
			else this.PauseGame();
			return this.paused;
		}

		public void PauseGame() {
			this.paused = true;
		}

		public void UnpauseGame() {
			this.paused = false;
		}

		public Level GetLevel() => this.level;
	}
}
