using SoulboundEngine.Common;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Common.Math.Random;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.Entity;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.World.Chunk;
using SoulboundEngine.World.Entity;
using SoulboundEngine.World.Gen;
using SoulboundEngine.World.Physics;
using SoulboundEngine.World.Player;
using SoulboundEngine.World.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.World.Level {
	using Block = Block.Block;
	using Chunk = Chunk.Chunk;
	using Entity = Entity.Entity;

	public sealed class Level : IHeightLimitView, ILevelExecutionService, IEntityManager {
		public const int CHUNK_LENGTH = SharedConstants.CHUNK_WIDTH;
		public const int WORLD_HEIGHT = 1024;
		public const int MIN_Y = -WORLD_HEIGHT / 2;
		public const int MAX_Y = WORLD_HEIGHT / 2;
		public const int RENDER_DISTANCE = 8;
		private const int CHUNK_TTL = 750;

		public readonly int seed;
		private readonly ChunkStorage chunkStorage;
		private readonly LevelChunkManager chunkManager;
		private readonly RandomSequences randomSequences;
		private PlayerEntity player = null!;
		public event Action<BlockPos, BlockState?, BlockState?>? blockStateChanged;
		public event Action<Entity>? entityAdded;
		public event Action<Entity>? entityRemoved;
		public event Action<Chunk>? chunkLoaded;
		public event Action<Chunk>? chunkUnloaded;
		private bool isLoaded;
		private bool levelActive;

		private readonly HashSet<BlockPos> tickingBlocks = new();
		private readonly Dictionary<Guid, Entity> entities = new();
		private readonly List<ITickingEntity> tickingEntities = new();

		public Level(int seed, ChunkGenerator chunkGenerator, int chunkRadius, ChunkStorage chunkStorage) {
			this.seed = seed;
			this.chunkStorage = chunkStorage;
			this.randomSequences = new RandomSequences(seed);
			this.chunkManager = new LevelChunkManager(this, chunkGenerator, chunkRadius, new LevelChunkCache(this, CHUNK_TTL), chunkStorage);
		}

		// known issue: current chunk generation takes way too long (60-65ms per chunk in one tick)
		public void GenerateSpawn(bool placeBlocks) {
			Logger.LogInfo("Generating terrain with seed {}", this.seed);
			this.chunkManager.InitialLoad(0, placeBlocks);
			this.isLoaded = true;
		}

		// known issue: player creation assumes block placement is finished
		public void StartSession(PlayerEntity player) {
			if (!this.isLoaded) {
				throw new InvalidOperationException("Cannot start world session without initial load");
			}
			this.levelActive = true;
			this.player = player;
			this.AddEntity(player);
			player.SetPos(this.GetWorldSpawnPoint() + Vec2d.UNIT_Y * 2.0d);
		}

		// known issue: inconsistent world update loop design
		public void Tick(RectInt simulationRect) {
			if (!this.IsLevelActive()) throw new InvalidOperationException("Cannot tick without an active session");

			foreach (var pos in this.tickingBlocks.ToArray()) {
				Vector2Int p = new(pos.x, pos.y);
				if (!simulationRect.Contains(p)) continue;

				BlockState blockState = this.GetBlockState(pos);
				((ITickingBlock)blockState.block).Tick(this, pos, blockState);
			}

			foreach (var entity in this.GetAllEntities()) {
				Vec2i p = entity.GetPosition().FloorToInt();
				if (simulationRect.Contains(new Vector2Int(p.x, p.y))) {
					entity.Tick();
				}
			}

			this.chunkManager.SetCenterX(ChunkXAt(this.player.GetPosition()));
			this.chunkManager.Tick(true);
		}

		public Vec2d GetWorldSpawnPoint() {
			return new Vec2d(0f, this.GetSurfaceAirY(0));
		}

		[PROTOTYPICAL]
		public void SetBlockState(BlockPos blockPos, BlockState blockState) {
			Chunk? chunk = this.ChunkAt(blockPos);
			if (chunk == null) {
				Logger.LogError("Block pos not valid: " + blockPos);
				return;
			}
			BlockState? oldState = this.GetBlockState(blockPos);

			oldState?.OnStateReplaced(blockPos, this);
			chunk.SetBlockState(blockPos, blockState);
			blockStateChanged?.Invoke(blockPos, oldState, blockState);

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
		}

		private void NotifyNeighboringStates(BlockPos blockPos) {
			foreach (var neighborPos in blockPos.GetCardinalNeighbors()) {
				Chunk? chunk = this.ChunkAt(blockPos);
				if (chunk == null) return;

				BlockState? blockState = this.GetBlockState(neighborPos);
				Block block = blockState?.block ?? Blocks.AIR;

				if (block is INeighborUpdateHandler neighborUpdateHandler) {
					neighborUpdateHandler.OnNeighborChanged(this, neighborPos, blockPos);
				}
			}
		}

		public void AddEntity(Entity entity) {
			if (!this.IsLevelActive()) return;

			Guid guid = Guid.NewGuid();
			entity.OnAdd(guid);
			this.entities[guid] = entity;

			if (entity is ITickingEntity ticking) {
				this.tickingEntities.Add(ticking);
			}
			entityAdded?.Invoke(entity);
		}

		public void RemoveEntity(Entity entity) {
			if (!this.entities.ContainsKey(entity.guid)) return;

			this.entities.Remove(entity.guid);
			entity.Dispose();

			if  (entity is ITickingEntity ticking) {
				this.tickingEntities.Remove(ticking);
			}
			entityRemoved?.Invoke(entity);
		}

		public void SpawnEntity<E>(EntityDescriptor<E> descriptor, Vec2d pos) where E : Entity {
			if (!this.IsLevelActive()) return;
			descriptor.Create(this, pos);
		}

		void ILevelExecutionService.SpawnEntity(EntityDescriptor descriptor, Vec2d pos) {
			if (!this.IsLevelActive()) return;
			descriptor.CreateBoxed(this, pos);
		}

		public bool TryGetEntity(Guid guid, out Entity entity) {
			return this.entities.TryGetValue(guid, out entity);
		}

		/// <summary> Tries to get the closest entity at <c>worldPos</c> </summary>
		public bool TryGetEntityAt(Vec2d worldPos, out Entity entity) {
			entity = null!;
			double closestDist = double.MaxValue;

			// linear scan over the entire entity list is fine to start
			// if entity counts start becoming a bottleneck, switch to spatial hash or quadtree
			// but for now its too much of a premature abstraction
			foreach (var ent in this.entities.Values) {
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
			if (testBox.GetSize() < 1.0E-7) return new List<AABB>();
			return new BlockCollisionResolver(this, testBox);
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
			foreach (var entity in this.GetAllEntities()) {
				if (entity != except && selector(entity)) {
					output.Add(entity);
				}
			}
			return output;
		}

		public void OnChunkLoaded(Chunk chunk) {
			this.chunkLoaded?.Invoke(chunk);
		}

		public void OnChunkUnloaded(Chunk chunk) {
			this.chunkUnloaded?.Invoke(chunk);
		}

		public void DropChunk(Chunk chunk) {
			this.chunkStorage.Save(this, chunk);
		}

		public void OnSessionStop() {
			this.chunkManager.Dispose();
		}

		public BlockState GetBlockState(BlockPos blockPos) {
			if (!this.IsInHeightLimit(blockPos.y)) return Blocks.AIR.DefaultState;

			Chunk? chunk = this.ChunkAt(blockPos);
			return chunk?.GetBlockState(blockPos) ?? Blocks.AIR.DefaultState;
		}

		public TileEntity? GetTileEntity(BlockPos blockPos) {
			Chunk? chunk = this.ChunkAt(blockPos);
			return chunk?.GetTileEntity(blockPos);
		}

		public Block GetBlock(BlockPos blockPos) {
			BlockState blockState = this.GetBlockState(blockPos);
			return blockState.GetBlock();
		}

		public Func<BlockStateContainer> BlockStateContainerFactory() {
			return () => new BlockStateContainer(ChunkSection.WIDTH, ChunkSection.HEIGHT);
		}

		public static int ChunkXAt(Vec2d worldPos) => ChunkXAt(worldPos.x);
		public static int ChunkXAt(int x) => ChunkXAt((float)x);
		public static int ChunkXAt(double x) => Maths.FloorToInt(x / CHUNK_LENGTH);

		public static int ToWorldX(int cx, int chunkX) => cx + chunkX * CHUNK_LENGTH;
		public static int ToChunkX(int x) => x - ChunkXAt(x) * CHUNK_LENGTH;

		public Chunk? ChunkAt(int worldX) {
			return this.chunkManager.GetChunk(ChunkXAt(worldX), false);
		}
		public Chunk? ChunkAt(BlockPos blockPos) => this.ChunkAt(blockPos.x);

		public int GetBottomY() => MIN_Y;
		public int GetHeight() => WORLD_HEIGHT;

		public Chunk? GetChunk(int chunkX) => this.chunkManager.GetChunk(chunkX, false);

		public IEnumerable<Chunk> GetLoadedChunks() {
			return this.chunkManager.GetLoadedChunks();
		}

		public static bool IsInBounds(BlockPos pos) {
			return pos.y < WORLD_HEIGHT && pos.y >= MIN_Y;
		}

		public int GetSurfaceY(int xpos) {
			Chunk? chunk = this.ChunkAt(xpos);
			int cx = ToChunkX(xpos);
			return (chunk as WorldChunk)?.surfacePoints?[cx] ?? 0;
		}

		public int GetSurfaceAirY(int xpos) => this.GetSurfaceY(xpos) + 1;

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

		public PlayerEntity GetPlayer() => this.player;

		public RandomSequences RandomSequences => this.randomSequences;
	}
}
