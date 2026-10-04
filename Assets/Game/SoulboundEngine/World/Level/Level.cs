namespace SoulboundEngine.World.Level {
#nullable enable
	using Block;
	using Block.Entity;
	using Block.State;
	using Chunk;
	using Common.Math;
	using Common.Math.Random;
	using Entity;
	using Gen.Biome;
	using Physics;
	using Player;
	using Recipe;
	using Registry;
	using Serialization;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Widget;

	public sealed class Level : IWorldGenLevel, IEntityQueriable<Entity> {
		public const int CHUNK_LENGTH = SharedConstants.CHUNK_WIDTH;
		public const int DEFAULT_WORLD_HEIGHT = 1024;
		public const int DEFAULT_MIN_Y = -DEFAULT_WORLD_HEIGHT / 2;
		public const int DEFAULT_MAX_Y = DEFAULT_WORLD_HEIGHT / 2;
		public const int RENDER_DISTANCE = 8;
		private const int CHUNK_TTL = 750;
		private readonly RegistryEntry<LevelType> levelType;
		private readonly LevelSettings levelSettings;
		private readonly long seed;
		private readonly ChunkStorage chunkStorage;
		private readonly LevelChunkManager chunkManager;
		private readonly RandomSequences randomSequences;
		private readonly IRegistryManager registryManager;

		// recipes should technically be on "server"
		// but Level is currently the only source of truth
		private readonly RecipeManager recipeManager;
		private PlayerEntity player = null!;
		public event Action<BlockPos, BlockState, BlockState>? blockStateChanged;
		public event Action<Entity>? entityAdded;
		public event Action<Entity>? entityRemoved;
		public event Action<Chunk>? chunkLoaded;
		public event Action<Chunk>? chunkUnloaded;
		public event Action<WorldWidgetHandler>? widgetAdded;
		public event Action<WorldWidgetHandler>? widgetRemoved;
		private bool isLoaded;
		private bool levelActive;

		private readonly HashSet<BlockPos> tickingBlocks = new();
		private readonly Dictionary<Guid, Entity> entities = new();
		private readonly Dictionary<BlockPos, List<WorldWidgetHandler>> widgets = new();

		public Level(
			RegistryEntry<LevelType> levelType,
			LevelSettings levelSettings,
			IRegistryManager registryManager,
			long seed,
			RecipeManager recipeManager,
			int chunkRadius,
			ChunkStorage chunkStorage
		) {
			this.levelType = levelType;
			this.levelSettings = levelSettings;
			this.registryManager = registryManager;
			this.seed = seed;
			this.recipeManager = recipeManager;
			this.chunkStorage = chunkStorage;
			this.randomSequences = new RandomSequences(seed);
			this.chunkManager = new LevelChunkManager(this, levelSettings.chunkGenerator, chunkRadius, new LevelChunkCache(this, CHUNK_TTL), chunkStorage);
		}

		public void GenerateSpawn() {
			Logger.LogInfo("Generating terrain with seed {}", this.seed);
			this.chunkManager.InitialLoad(0);
			this.isLoaded = true;
		}

		public void DeserializeEntities(EntitySerializer entitySerializer) {
			foreach (Entity entity in entitySerializer.LoadAll(this)) {
				this.AddEntity(entity, entity.guid);
			}
		}

		public void StartSession(PlayerEntity player) {
			if (!this.isLoaded) {
				throw new InvalidOperationException("Cannot start world session without initial load");
			}
			this.levelActive = true;
			this.player = player;
			this.AddEntity(player, player.guid);
		}

		public void Tick(AABB simulationRect) {
			if (!this.IsLevelActive()) throw new InvalidOperationException("Cannot tick without an active session");

			foreach (BlockPos pos in this.tickingBlocks.ToArray()) {
				Vec2d p = new(pos.x, pos.y);
				if (!simulationRect.Contains(p)) continue;

				BlockState blockState = this.GetBlockState(pos);
				((ITickingBlock)blockState.block).Tick(this, pos, blockState);
			}

			foreach (Entity entity in this.GetAllEntities()) {
				Vec2i p = entity.GetPosition().FloorToInt();
				if (simulationRect.Contains(new Vec2d(p.x, p.y))) {
					entity.Tick();
				}
			}

			int chunkPos = SectionPos.BlockToSectionCoord(Maths.FloorToInt(this.player.GetX()));
			this.chunkManager.SetCenterX(chunkPos);
			this.chunkManager.Tick(true);
		}

		public Vec2d GetWorldSpawnPoint() {
			return new Vec2d(0f, this.GetHeight(0));
		}

		public bool SetBlockState(BlockPos blockPos, BlockState blockState) {
			if (this.IsOutOfHeightLimit(blockPos)) return false;
			Chunk? chunk = this.GetChunk(blockPos);
			if (chunk == null) {
				Logger.LogError("Block pos not valid: " + blockPos);
				return false;
			}
			BlockState oldState = this.GetBlockState(blockPos);

			oldState.OnStateReplaced(blockPos, this);
			chunk.SetBlockState(blockPos, blockState);
			this.blockStateChanged?.Invoke(blockPos, oldState, blockState);

			bool oldTicks = oldState?.block is ITickingBlock;
			bool newTicks = blockState?.block is ITickingBlock;
			if (oldTicks) this.tickingBlocks.Remove(blockPos);
			if (newTicks) this.tickingBlocks.Add(blockPos);

			// neighbor updates arent dispatched for a block that has just been placed
			// so we manually update the block through another neighbor update
			// this isnt entirely correct, but for the sake of simplicity it works for now
			if (blockState?.block is INeighborUpdateHandler neighborUpdateHandler) {
				neighborUpdateHandler.OnNeighborChanged(this, blockPos, blockPos);
			}

			this.NotifyNeighboringStates(blockPos);
			return true;
		}

		public bool RemoveBlock(BlockPos blockPos) {
			return this.SetBlockState(blockPos, Blocks.AIR.DefaultState);
		}

		public bool IsStateAtPosition(BlockPos blockPos, Predicate<BlockState> predicate) {
			return predicate(this.GetBlockState(blockPos));
		}

		private void NotifyNeighboringStates(BlockPos blockPos) {
			foreach (BlockPos neighborPos in blockPos.GetCardinalNeighbors()) {
				Chunk? chunk = this.GetChunk(blockPos);
				if (chunk == null) return;

				BlockState? blockState = this.GetBlockState(neighborPos);
				Block block = blockState?.block ?? Blocks.AIR;

				if (block is INeighborUpdateHandler neighborUpdateHandler) {
					neighborUpdateHandler.OnNeighborChanged(this, neighborPos, blockPos);
				}
			}
		}

		public int GetHeight(int blockX) {
			if (!this.HasChunk(SectionPos.BlockToSectionCoord(blockX))) return this.GetBottomY();

			Chunk chunk = this.GetChunk(SectionPos.BlockToSectionCoord(blockX))!;
			return chunk.GetHeight(chunk.pos.ToLocalX(blockX));
		}

		public bool AddNewEntity(Entity entity) {
			Guid guid = Guid.NewGuid();
			return this.AddEntity(entity, guid);
		}

		public bool AddEntity(Entity entity, Guid guid) {
			if (!this.entities.TryAdd(guid, entity)) return false;

			entity.OnAdd(guid);
			entity.SetAlive(true);
			this.entityAdded?.Invoke(entity);
			return true;
		}

		[Obsolete]
		public void RemoveEntity(Entity entity) {
			if (!this.entities.ContainsKey(entity.guid)) return;

			this.entities.Remove(entity.guid);
			entity.Dispose();
			this.entityRemoved?.Invoke(entity);
		}

		public bool SpawnEntity<E>(EntityDescriptor<E> descriptor, Vec2d pos) where E : Entity {
			E? entity = descriptor.Create(this, pos);
			if (entity != null) {
				this.AddNewEntity(entity);
				return true;
			}
			return false;
		}

		public bool SpawnEntity(EntityDescriptor descriptor, Vec2d pos) {
			Entity? entity = descriptor.Create(this);
			if (entity != null) {
				entity.SetPos(pos);
				this.AddNewEntity(entity);
				return true;
			}
			return false;
		}


		public Entity? GetEntity(Guid guid) => this.entities.GetValueOrDefault(guid);

		/// <summary> Tries to get the closest entity at <c>worldPos</c> </summary>
		public bool TryGetEntityAt(Vec2d worldPos, out Entity entity) {
			entity = null!;
			double closestDist = double.MaxValue;

			// linear scan over the entire entity list is fine to start
			// if entity counts start becoming a bottleneck, switch to spatial hash or quadtree
			// but for now its too much of a premature abstraction
			foreach (Entity ent in this.entities.Values) {
				if (!ent.boundingBox.Contains(worldPos)) continue;

				double dist = Vec2d.Distance(worldPos, ent.boundingBox.GetCenter());
				if (dist < closestDist) {
					entity = ent;
					closestDist = dist;
				}
			}

			return entity != null;
		}

		public IEnumerable<Entity> GetAllEntities() => this.entities.Values.ToList();

		public IEnumerable<AABB> GetBlockCollisionBoxes(AABB testBox) {
			return testBox.GetSize() < 1.0E-7 ? new List<AABB>() : new BlockCollisionResolver(this, testBox);
		}

		public IEnumerable<AABB> GetEntityCollisions(Entity? source, AABB testBox) {
			if (testBox.GetSize() < 1.0E-7) return new List<AABB>();

			Predicate<Entity> canCollide = source == null ? Entity.CAN_BE_COLLIDED_WITH : e => e.CanBeCollidedWith(source);
			List<Entity> collidingEntities = this.GetEntities(source, testBox.Stretch(1.0E-7), canCollide);
			if (collidingEntities.Count == 0) return new List<AABB>();

			List<AABB> colliders = new();
			foreach (Entity entity in collidingEntities) {
				colliders.Add(entity.boundingBox);
			}
			return colliders;
		}

		public List<Entity> GetEntities(Entity? except, AABB box, Predicate<Entity> selector) {
			return this.GetEntities(except, e => e.boundingBox.Overlaps(box) && selector(e));
		}

		public List<Entity> GetEntities(Entity? except, Predicate<Entity> selector) {
			List<Entity> output = new();
			foreach (Entity entity in this.GetAllEntities()) {
				if (entity != except && selector(entity)) {
					output.Add(entity);
				}
			}
			return output;
		}

		public WorldWidgetHandler<TContext> AddWidget<TContext>(
			IWorldWidgetProvider<TContext> widgetProvider,
			Func<Level, BlockPos, TContext> contextFactory,
			BlockPos pos
		) where TContext : WorldWidgetContext {
			TContext context = contextFactory(this, pos);
			WorldWidgetHandler<TContext> handler = widgetProvider.CreateHandler(context);

			if (!this.widgets.ContainsKey(pos)) {
				this.widgets[pos] = new List<WorldWidgetHandler>();
			}
			this.widgets[pos].Add(handler);
			this.widgetAdded?.Invoke(handler);

			return handler;
		}

		public void RemoveWidget(WorldWidgetHandler handler) {
			this.RemoveWidget(handler.GetContext().blockPos, handler);
		}

		public void RemoveWidget(BlockPos pos, WorldWidgetHandler handler) {
			if (!this.widgets.ContainsKey(pos)) return;

			List<WorldWidgetHandler> handlers = this.widgets[pos];
			if (!handlers.Remove(handler)) return;

			if (handlers.Count == 0) this.widgets.Remove(pos);
			this.widgetRemoved?.Invoke(handler);
		}

		public bool RemoveAllWidgetsAt(BlockPos pos) {
			if (this.widgets.Remove(pos, out List<WorldWidgetHandler> list)) {
				foreach (WorldWidgetHandler handler in list) {
					this.widgetRemoved?.Invoke(handler);
				}
				return true;
			}
			return false;
		}

		public IEnumerable<WorldWidgetHandler> GetWidgets(BlockPos pos) {
			if (this.widgets.TryGetValue(pos, out List<WorldWidgetHandler> handlers)) {
				foreach (WorldWidgetHandler handler in handlers) {
					yield return handler;
				}
			}
		}

		public IEnumerable<WorldWidgetHandler> GetAllWidgets() {
			foreach ((BlockPos pos, List<WorldWidgetHandler> handlers) in this.widgets) {
				foreach (WorldWidgetHandler handler in handlers) {
					yield return handler;
				}
			}
		}

		public void OnChunkLoaded(Chunk chunk) {
			this.chunkLoaded?.Invoke(chunk);
		}

		public void OnChunkUnloaded(Chunk chunk) {
			this.chunkUnloaded?.Invoke(chunk);
		}

		public void DropChunk(Chunk chunk) {
			this.chunkStorage.Save(chunk);
		}

		public void OnSessionStop() {
			this.chunkManager.Dispose();
		}

		public ChunkManager GetChunkManager() => this.chunkManager;

		public IRegistryManager GetRegistries() => this.registryManager;

		public BlockState GetBlockState(BlockPos blockPos) {
			if (!this.IsInHeightLimit(blockPos.y)) return Blocks.AIR.DefaultState;

			Chunk? chunk = this.GetChunk(blockPos);
			return chunk?.GetBlockState(blockPos) ?? Blocks.AIR.DefaultState;
		}

		public TileEntity? GetTileEntity(BlockPos blockPos) {
			Chunk? chunk = this.GetChunk(blockPos);
			return chunk?.GetTileEntity(blockPos);
		}

		public Block GetBlock(BlockPos blockPos) {
			BlockState blockState = this.GetBlockState(blockPos);
			return blockState.GetBlock();
		}

		public RegistryEntry<Biome> GetBiome(BlockPos blockPos) {
			int chunkX = SectionPos.BlockToSectionCoord(blockPos.x);
			return this.GetChunk(chunkX).GetBiome(blockPos.ToChunkPos().xInChunk);
		}

		public Func<BlockStateContainer> BlockStateContainerFactory() {
			return () => new BlockStateContainer(ChunkSection.WIDTH, ChunkSection.HEIGHT);
		}

		public int GetBottomY() => DEFAULT_MIN_Y;

		public int GetHeight() => DEFAULT_WORLD_HEIGHT;

		public Chunk? GetChunk(int chunkX, bool loadOrGenerate) {
			return this.chunkManager.GetChunk(chunkX, loadOrGenerate);
		}

		public Chunk? GetChunk(BlockPos blockPos) {
			return this.GetChunk(SectionPos.BlockToSectionCoord(blockPos.x));
		}

		public Chunk? GetChunk(int chunkPos) {
			return this.chunkManager.GetChunk(chunkPos, true);
		}

		public IEnumerable<Chunk> GetLoadedChunks() {
			return this.chunkManager.GetLoadedChunks();
		}

		public List<BlockPos> GetTilesCovered(AABB bounds) {
			List<BlockPos> coveredTiles = new();
			Vec2i min = bounds.Min.FloorToInt();
			Vec2i max = bounds.Max.FloorToInt();

			for (int x = min.x; x <= max.x; x++) {
				for (int y = min.y; y <= max.y; y++) {
					coveredTiles.Add(new BlockPos(x, y));
				}
			}
			return coveredTiles;
		}

		public bool IsLevelActive() => this.levelActive;

		public bool IsLoaded() => this.isLoaded;

		public long GetSeed() => this.seed;

		public PlayerEntity GetPlayer() => this.player;

		public LevelType GetLevelType() => this.levelType.GetValue();

		public LevelSettings GetSettings() => this.levelSettings;

		public RandomSequences RandomSequences => this.randomSequences;

		public RecipeManager RecipeManager => this.recipeManager;
	}
}