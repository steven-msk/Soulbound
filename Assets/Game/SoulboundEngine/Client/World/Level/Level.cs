using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.Runtime.Services;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Client.World.Entity;
using SoulboundEngine.Client.World.Gen;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Common.Math.Random;
using SoulboundEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.Client.World.Level {
	using Block = Block.Block;
	using Chunk = Chunk.Chunk;
	using Entity = Entity.Entity;

	public sealed class Level : IHeightLimitView, ILevelExecutionService, IEntityManager {
		public delegate void OnChunkGenerated(ChunkGenData genData);
		public const int CHUNK_LENGTH = SharedConstants.CHUNK_WIDTH;
		public const int WORLD_HEIGHT = 1024;
		public const int MIN_Y = -WORLD_HEIGHT / 2;
		public const int MAX_Y = WORLD_HEIGHT / 2;
		public const int RENDER_DISTANCE = 8;
		private const int CHUNK_TICKS_TO_LIVE = 3000;

		public readonly int seed;
		private readonly ChunkStorage chunkStorage;
		private readonly LevelChunkManager chunkManager;
		private readonly Dictionary<int, ChunkGenData> chunkGenData = new();
		private readonly RandomSequences randomSequences;
		private PlayerEntity player = null!;
		public event Action<BlockPos, BlockState?, BlockState?>? blockStateChanged;
		public event Action<Entity>? entityAdded;
		public event Action<Entity>? entityRemoved;
		public event Action<Chunk>? chunkLoaded;
		public event Action<Chunk>? chunkUnloaded;

		private readonly HashSet<BlockPos> tickingBlocks = new();
		private readonly Dictionary<Guid, Entity> entities = new();
		private readonly List<ITickingEntity> tickingEntities = new();

		public Level(int seed, ChunkGenerator chunkGenerator, int chunkRadius, ChunkStorage chunkStorage) {
			this.seed = seed;
			this.chunkStorage = chunkStorage;
			this.randomSequences = new RandomSequences(seed);
			this.chunkManager = new LevelChunkManager(this, chunkGenerator, chunkRadius, new LevelChunkCache(this, CHUNK_TICKS_TO_LIVE), chunkStorage);
		}

		// known issue: current chunk generation takes way too long (60-65ms per chunk in one tick)
		public void GenerateSpawn(bool placeBlocks) {
			Logger.LogInfo("Generating terrain with seed {}", this.seed);
			this.chunkManager.InitialLoad(0, placeBlocks);
		}

		// known issue: player creation assumes block placement is finished
		public void StartSession(PlayerEntity player) {
			this.player = player;
			this.AddEntity(player);
			player.SetPosition(this.GetWorldSpawnPoint() + Vector2.up * 2f);
		}

		// known issue: inconsistent world update loop design
		public void Tick(RectInt simulationRect) {
			foreach (var pos in this.tickingBlocks.ToArray()) {
				if (!simulationRect.Contains((Vector2Int)pos)) continue;

				BlockState blockState = this.GetBlockState(pos);
				((ITickingBlock)blockState.block).Tick(this, pos, blockState);
			}

			foreach (var entity in this.GetAllEntities()) {
				if (simulationRect.Contains(Vector2Int.FloorToInt(entity.GetPosition()))) {
					entity.Tick();
				}
			}

			this.chunkManager.SetCenterX(ChunkXAt(this.player.GetPosition()));
			this.chunkManager.Tick(true);
		}

		public Vector2 GetWorldSpawnPoint() {
			return new Vector2(0f, this.GetSurfaceAirY(0));
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

		public void SpawnEntity<E>(EntityDescriptor<E> descriptor, Vector2 pos) where E : Entity {
			descriptor.Create(this, pos);
		}

		void ILevelExecutionService.SpawnEntity(EntityDescriptor descriptor, Vector2 pos) {
			descriptor.CreateBoxed(this, pos);
		}

		public bool TryGetEntity(Guid guid, out Entity entity) {
			return this.entities.TryGetValue(guid, out entity);
		}

		/// <summary> Tries to get the closest entity at <c>worldPos</c> </summary>
		public bool TryGetEntityAt(Vector2 worldPos, out Entity entity) {
			entity = null!;
			float closestDist = float.MaxValue;

			// linear scan over the entire entity list is fine to start
			// if entity counts start becoming a bottleneck, switch to spatial hash or quadtree
			// but for now its too much of a premature abstraction
			foreach (var ent in this.entities.Values) {
				if (!ent.GetBoundingBox().Contains(worldPos)) continue;

				float dist = Vector2.Distance(worldPos, ent.GetCenter());
				if (dist < closestDist) {
					entity = ent;
					closestDist = dist;
				}
			}

			return entity != null;
		}

		public IEnumerable<Entity> GetAllEntities() => this.entities.Values.ToList();

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

		public static int ChunkXAt(Vector2 worldPos) => ChunkXAt(worldPos.x);
		public static int ChunkXAt(int x) => ChunkXAt((float)x);
		public static int ChunkXAt(float x) => Mathf.FloorToInt(x / CHUNK_LENGTH);

		public static int ToWorldX(int cx, int chunkX) => cx + chunkX * CHUNK_LENGTH;
		public static int ToChunkX(int x) => x - ChunkXAt(x) * CHUNK_LENGTH;

		public Chunk? ChunkAt(int worldX) => this.ChunkAt(ChunkXAt(worldX));
		public Chunk? ChunkAt(BlockPos blockPos) {
			return this.chunkManager.GetChunk(ChunkXAt(blockPos.x), false);
		}

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
			int chunkX = ChunkXAt(xpos);
			int cx = ToChunkX(xpos);

			return this.chunkGenData.TryGetValue(chunkX, out var value)
				? value.surfacePoints[cx]
				: 0;
		}

		public int GetSurfaceAirY(int xpos) => this.GetSurfaceY(xpos) + 1;

		public List<BlockPos> GetTilesCovered(Bounds bounds) {
			List<BlockPos> coveredTiles = new();
			Vector2Int min = Vector2Int.FloorToInt(bounds.min);
			Vector2Int max = Vector2Int.FloorToInt(bounds.max);

			for (int x = min.x; x <= max.x; x++) {
				for (int y = min.y; y <= max.y; y++) {
					coveredTiles.Add(new BlockPos(x, y));
				}
			}
			return coveredTiles;
		}

		public PlayerEntity GetPlayer() => this.player;

		public RandomSequences RandomSequences => this.randomSequences;
	}
}
