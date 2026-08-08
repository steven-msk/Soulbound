using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.Runtime.Services;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Client.World.Entity;
using SoulboundEngine.Client.World.Generation;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Common.Math.Random;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

#nullable enable

namespace SoulboundEngine.Client.World.Level {
	using Block = Block.Block;
	using Entity = Entity.Entity;

	public sealed class Level : ILevelExecutionService, IEntityManager {
		public delegate void OnChunkGenerated(ChunkGenData genData);
		public const int CHUNK_LENGTH = 32;
		public const int WORLD_HEIGHT = 1024;
		public const int RENDER_DISTANCE = 8;
		public const int TERRAIN_PLANE_Y = 0;
		const int biomeBlendRange = 10;

		public readonly int seed;
		private readonly Dictionary<int, WorldChunk> loadedChunks = new();
		private readonly Dictionary<int, WorldChunk> generatedChunks = new(); 
		[Obsolete] private readonly ConcurrentDictionary<int, List<OnChunkGenerated>> deferredGenerations = new();
		private readonly Dictionary<int, ChunkGenData> chunkGenData = new();
		private readonly RandomSequences randomSequences;
		private PlayerEntity player;
		public event Action<BlockPos, BlockState?, BlockState?>? blockStateChanged;
		public event Action<Entity>? entityAdded;
		public event Action<Entity>? entityRemoved;
		public event Action<WorldChunk>? chunkLoaded;
		public event Action<WorldChunk>? chunkUnloaded;

		private readonly BiomeMap biomeMap;
		private readonly Heightmap heightmap;
		private readonly Cavemap cavemap;

		private readonly HashSet<BlockPos> tickingBlocks = new();
		private readonly Dictionary<Guid, Entity> entities = new();
		private readonly List<ITickingEntity> tickingEntities = new();

		public Level(int seed) {
			this.seed = seed;
			this.randomSequences = new RandomSequences(seed);

			var biome1 = new PlainsBiome(seed);
			var biome2 = new HillsBiome(seed);
			this.biomeMap = new BiomeMap(new IBiome[] { biome1, biome2 });
			this.heightmap = new Heightmap(TERRAIN_PLANE_Y);
			this.cavemap = new Cavemap(seed);
		}

		// known issue: current chunk generation takes way too long (60-65ms per chunk in one tick)
		public void GenerateInitialTerrain(bool placeBlocks) {
			Logger.LogInfo("Generating terrain with seed {}", this.seed);
			for (int cx = -RENDER_DISTANCE; cx <= RENDER_DISTANCE; cx++) {
				this.GenerateNewChunk(cx, placeBlocks);
			}
		}

		public void ApplyDeserializedBlocks(Dictionary<int, int[][]> stateIDsByChunk) {
			foreach (var (chunkX, stateIDs) in stateIDsByChunk) {
				if (!this.generatedChunks.TryGetValue(chunkX, out WorldChunk chunk)) {
					chunk = this.GenerateNewChunk(chunkX, false);
					this.generatedChunks[chunkX] = chunk;
				}
				chunk.SetAllBlocks(stateIDs);
			}
		}

		public void ApplyDeserializedTileEntities(IEnumerable<TileEntity> tileEntities) {
			foreach (var tileEntity in tileEntities) {
				BlockPos blockPos = tileEntity.GetBlockPos();
				ChunkBlockPos chunkPos = blockPos.ToChunkPos();
				WorldChunk? chunk = this.GetChunk(chunkPos.chunkX);
				if (chunk == null) {
					Logger.LogError("Chunk {} is not generated, yet a TileEntity was deserialized in it: {} at {}",
						chunkPos.x, tileEntity, blockPos);
					continue;
				}

				if (!chunk.ValidateTileEntity(tileEntity)) continue;
				chunk.AddTileEntityValidated(tileEntity);
			}
		}

		public void SyncBlocksWithTileEntities() {
			foreach (var chunk in this.generatedChunks.Values) {
				chunk.SyncBlocksWithTileEntities();
			}
		}

		// known issue: player creation assumes block placement is finished
		public void StartSession(PlayerEntity player) {
			this.player = player;
			this.AddEntity(player);
			player.SetPosition(this.GetWorldSpawnPoint() + Vector2.up * 2f);
		}

		// known issue: inconsistent world update loop design
		public void Tick(RectInt simulationRect) {
			int pivotChunkX = ChunkXAt(this.player.GetPosition());
			// known issue: current chunk generation takes way too long (60-65ms per chunk in one tick)
			this.UnloadDistantChunks(pivotChunkX, RENDER_DISTANCE);
			this.UpdateLoadedChunks(pivotChunkX);

			foreach (var pos in this.tickingBlocks.ToArray()) {
				if (!simulationRect.Contains((Vector2Int)pos)) continue;

				BlockState? blockState = this.GetBlockState(pos);
				if (blockState == null) continue;

				((ITickingBlock)blockState.block).Tick(this, pos, blockState);
			}

			foreach (var entity in this.GetAllEntities()) {
				if (simulationRect.Contains(Vector2Int.FloorToInt(entity.GetPosition()))) {
					entity.Tick();
				}
			}

			foreach (var chunk in this.loadedChunks.Values) {
				chunk.Tick();
			}
		}

		public Vector2 GetWorldSpawnPoint() {
			return new Vector2(0f, this.GetSurfaceAirY(0));
		}

		private WorldChunk GenerateNewChunk(int chunkX, bool placeBlocks) {
			WorldChunk chunk = new(this, chunkX);
			this.generatedChunks[chunkX] = chunk;

			chunk.Generate(this.biomeMap, this.heightmap, this.cavemap, placeBlocks, out ChunkGenData genData);
			this.chunkGenData[chunkX] = genData;

			if (placeBlocks) this.BlendBiomeBorder(genData.biomePartition);

			this.HandleOnChunkGenerated(chunkX);
			if (placeBlocks) chunk.PostProcess(genData, this);

			return chunk;
		}

		[Obsolete]
		void BlendBiomeBorder(ChunkBiomePartition partition) {
			if (!partition.hasBorder) {
				return;
			}

			int leftX = partition.splitX - (biomeBlendRange / 2);
			int rightX = partition.splitX + (biomeBlendRange / 2) - 1;
			BlockResolver blockResolver = new(partition.primary, partition.secondary);

			for (int x = leftX; x <= rightX; x++) {
				int cx = ToChunkX(x);
				int chunkX = ChunkXAt(x);
				int localCx = cx;

				OnChunkGenerated onChunkGenerated = genData => {
					for (int y = 0; y < WORLD_HEIGHT; y++) {
						BlockGenContext ctx = genData.genContexts[localCx][y];
						var blockState = blockResolver.BlendBiomeBorder(ctx, leftX, rightX);
						genData.chunk.SetBlock(localCx, y, blockState);
					}
				};

				if (this.IsChunkGenerated(chunkX)) {
					onChunkGenerated(this.chunkGenData[chunkX]);
				} else {
					this.PostOnChunkGenerated(chunkX, onChunkGenerated);
				}
			}
		}

		[Obsolete]
		public void PostOnChunkGenerated(int chunkX, OnChunkGenerated onChunkGenerated) {
			if (!this.deferredGenerations.TryGetValue(chunkX, out var list)) {
				this.deferredGenerations[chunkX] = new List<OnChunkGenerated>();
			}
			list?.Add(onChunkGenerated);
		}

		[Obsolete]
		private void HandleOnChunkGenerated(int chunkX) {
			if (this.deferredGenerations.Remove(chunkX, out var list)) {
				var genData = this.chunkGenData[chunkX];

				foreach (var onChunkGenerated in list) {
					onChunkGenerated(genData);
				}
			}
		}

		[PROTOTYPICAL]
		public void SetBlockState(BlockPos blockPos, BlockState? blockState) {
			WorldChunk? chunk = this.ChunkAt(blockPos);
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
				WorldChunk? chunk = this.ChunkAt(blockPos);
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

		public void UnloadDistantChunks(int pivotChunkX, int viewDistance) {
			List<WorldChunk> toRemove = new();

			foreach (int chunkX in this.loadedChunks.Keys) {
				if (Mathf.Abs(chunkX - pivotChunkX) > viewDistance) {
					toRemove.Add(this.loadedChunks[chunkX]);
				}
			}

			foreach (WorldChunk chunk in toRemove) {
				this.loadedChunks.Remove(chunk.xpos);
				this.OnChunkUnloaded(chunk);
			}
		}

		public void UpdateLoadedChunks(int pivotChunkX) {
			for (int dx = -RENDER_DISTANCE; dx <= RENDER_DISTANCE; dx++) {
				int chunkX = pivotChunkX + dx;

				if (!this.loadedChunks.ContainsKey(chunkX)) {
					WorldChunk chunk;

					if (!this.generatedChunks.ContainsKey(chunkX)) {
						chunk = this.GenerateNewChunk(chunkX, true);
						this.generatedChunks[chunkX] = chunk;
					} else {
						chunk = this.generatedChunks[chunkX];
					}

					this.loadedChunks[chunkX] = chunk;
					this.OnChunkLoaded(chunk);
				}
			}
		}

		private void OnChunkLoaded(WorldChunk chunk) {
			this.chunkLoaded?.Invoke(chunk);
		}

		private void OnChunkUnloaded(WorldChunk chunk) {
			this.chunkUnloaded?.Invoke(chunk);
		}

		public void OnSessionStop() {
		}

		public BlockState? GetBlockState(BlockPos blockPos) {
			WorldChunk? chunk = this.ChunkAt(blockPos);
			return chunk?.GetBlockState(blockPos.ToChunkPos());
		}

		public TileEntity? GetTileEntity(BlockPos blockPos) {
			WorldChunk? chunk = this.ChunkAt(blockPos);
			return chunk?.TileEntityAt(blockPos);
		}

		public Block? GetBlock(BlockPos blockPos) {
			BlockState? blockState = this.GetBlockState(blockPos);
			return blockState?.block;
		}

		public static int ChunkXAt(Vector2 worldPos) => ChunkXAt(worldPos.x);
		public static int ChunkXAt(int x) => ChunkXAt((float)x);
		public static int ChunkXAt(float x) => Mathf.FloorToInt(x / CHUNK_LENGTH);

		public static int ToWorldX(int cx, int chunkX) => cx + chunkX * CHUNK_LENGTH;
		public static int ToChunkX(int x) => x - ChunkXAt(x) * CHUNK_LENGTH;

		public WorldChunk? ChunkAt(int xpos) => this.ChunkAt(new BlockPos(xpos, 0));
		public WorldChunk? ChunkAt(BlockPos blockPos) { 
			return this.generatedChunks!.GetValueOrDefault(ChunkXAt(blockPos.x), null);
		}

		public WorldChunk? GetChunk(int chunkX) {
			if (this.generatedChunks.TryGetValue(chunkX, out var chunk)) {
				return chunk;
			}
			return null;
		}

		public IEnumerable<WorldChunk> GetLoadedChunks() {
			return this.loadedChunks.Values.ToList();
		}

		public IEnumerable<WorldChunk> GetGeneratedChunks() {
			return this.generatedChunks.Values.ToList();
		}

		public static bool IsInBounds(BlockPos pos) {
			return pos.y < WORLD_HEIGHT && pos.y >= WorldChunk.minY;
		}

		public int GetSurfaceY(int xpos) {
			int chunkX = ChunkXAt(xpos);
			int cx = ToChunkX(xpos);

			return this.chunkGenData.TryGetValue(chunkX, out var value)
				? value.surfacePoints[cx]
				: 0;
		}

		public int GetSurfaceAirY(int xpos) => this.GetSurfaceY(xpos) + 1;

		public bool IsChunkLoaded(WorldChunk chunk) => this.loadedChunks.ContainsValue(chunk);

		public bool IsChunkLoaded(int chunkX) => this.loadedChunks.ContainsKey(chunkX);

		public bool IsChunkGenerated(int chunkX) => this.generatedChunks.ContainsKey(chunkX);

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
