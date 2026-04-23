using SoulboundEngine.Client.Players;
using SoulboundEngine.Client.Runtime.Services;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Client.World.BlockSystem.States;
using SoulboundEngine.Client.World.BlockSystem.TileEntities;
using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Client.World.EntitySystem;
using SoulboundEngine.Client.World.Generation;
using SoulboundEngine.Client.World.Render;
using SoulboundEngine.Common;
using SoulboundEngine.Common.Math;
using SoulboundEngine.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.World.LevelDomain {
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
		private readonly ChunkOutlineRenderer chunkOutlineRenderer = new();
		[Obsolete] private readonly ConcurrentDictionary<int, List<OnChunkGenerated>> deferredGenerations = new();
		private readonly Dictionary<int, ChunkGenData> chunkGenData = new();
		private readonly WorldRenderer worldRenderer;
		private Player player;

		private readonly BiomeMap biomeMap;
		private readonly Heightmap heightmap;
		private readonly Cavemap cavemap;

		private readonly HashSet<BlockPos> tickingBlocks = new();
		private readonly Dictionary<Guid, Entity> entities = new();
		private readonly List<ITickingEntity> tickingEntities = new();

		public Level(WorldRenderer worldRenderer, int seed) {
			this.seed = seed;
			this.worldRenderer = worldRenderer;

			var biome1 = new PlainsBiome(seed);
			var biome2 = new HillsBiome(seed);
			this.biomeMap = new BiomeMap(new IBiome[] { biome1, biome2 });
			this.heightmap = new Heightmap(TERRAIN_PLANE_Y);
			this.cavemap = new Cavemap(seed);
		}

		// known issue: world architecture design is poorly designed
		public void GenerateTerrain() {
			for (int cx = -RENDER_DISTANCE; cx <= RENDER_DISTANCE; cx++) {
				GenerateNewChunk(cx);
			}
		}

		// known issue: player creation assumes block placement is finished
		public void StartSession(Player player) {
			this.player = player;
			AddEntity(player);
			Soulbound.instance.GetInputManager().PushContext(player);
			player.SetPos(GetWorldSpawnPoint() + Vector2.up * 2f);
		}

		// known issue: inconsistent world update loop design
		public void Tick(RectInt simulationRect) {
			foreach (var pos in tickingBlocks.ToArray()) {
				if (!simulationRect.Contains((Vector2Int)pos)) continue;

				BlockState? blockState = GetBlockState(pos);
				if (blockState == null) continue;

				((ITickingBlock)blockState.block).Tick(this, pos, blockState);
			}

			foreach (var entity in tickingEntities.ToArray()) {
				if (simulationRect.Contains(Vector2Int.FloorToInt(((Entity)entity).GetPos()))) {
					entity.Tick();
				}
			}

			foreach (var chunk in loadedChunks.Values) {
				chunk.Tick();
			}
		}

		public Vector2 GetWorldSpawnPoint() {
			return new Vector2(0f, GetSurfaceAirY(0));
		}

		// known issue: inconsistent world update loop design
		public void FrameUpdate() {
			int pivotChunkX = ChunkXAt(player.GetPos());
			this.UnloadDistantChunks(pivotChunkX, RENDER_DISTANCE);

			for (int dx = -RENDER_DISTANCE; dx <= RENDER_DISTANCE; dx++) {
				int chunkX = pivotChunkX + dx;

				if (!loadedChunks.ContainsKey(chunkX)) {
					WorldChunk chunk;

					if (!generatedChunks.ContainsKey(chunkX)) {
						chunk = this.GenerateNewChunk(chunkX);
						generatedChunks[chunkX] = chunk;
					} else {
						chunk = generatedChunks[chunkX];
					}

					loadedChunks[chunkX] = chunk;
					chunk.OnLoad(chunkOutlineRenderer);
				}
			}

			foreach (var entity in GetAllEntities().ToArray()) {
				entity.FrameUpdate();
			}
		}

		private WorldChunk GenerateNewChunk(int chunkX) {
			WorldChunk chunk = new(this, chunkX);
			generatedChunks[chunkX] = chunk;

			chunk.Generate(biomeMap, heightmap, cavemap, out ChunkGenData genData);
			chunkGenData[chunkX] = genData;

			BlendBiomeBorder(genData.biomePartition);

			HandleOnChunkGenerated(chunkX);
			chunk.PostProcess(genData, this);

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

				if (IsChunkGenerated(chunkX)) {
					onChunkGenerated(chunkGenData[chunkX]);
				} else {
					PostOnChunkGenerated(chunkX, onChunkGenerated);
				}
			}
		}

		[Obsolete]
		public void PostOnChunkGenerated(int chunkX, OnChunkGenerated onChunkGenerated) {
			if (!deferredGenerations.TryGetValue(chunkX, out var list)) {
				deferredGenerations[chunkX] = new List<OnChunkGenerated>();
			}
			list.Add(onChunkGenerated);
		}

		[Obsolete]
		private void HandleOnChunkGenerated(int chunkX) {
			if (deferredGenerations.Remove(chunkX, out var list)) {
				var genData = chunkGenData[chunkX];

				foreach (var onChunkGenerated in list) {
					onChunkGenerated(genData);
				}
			}
		}

		[PROTOTYPICAL]
		public void SetBlockState(BlockPos blockPos, BlockState? blockState) {
			BlockState? oldState = GetBlockState(blockPos);
			WorldChunk chunk = ChunkAt(blockPos)
				?? throw new InvalidOperationException("Block pos not valid: " + blockPos);

			chunk.SetBlockState(blockPos, blockState);
			worldRenderer.UpdateModel(blockPos, blockState);

			bool oldTicks = oldState?.block is ITickingBlock;
			bool newTicks = blockState?.block is ITickingBlock;
			if (oldTicks) tickingBlocks.Remove(blockPos);
			if (newTicks) tickingBlocks.Add(blockPos);

			// neighbor updates arent dispatched for a block that has just been placed
			// so we manually update the block through another neighbor update
			// this isnt entirely correct, but for the sake of simplicity it works for now
			if (blockState?.block is INeighborUpdateHandler neighborUpdateHandler) {
				neighborUpdateHandler.OnNeighborChanged(this, blockPos, blockPos);
			}

			NotifyNeighboringStates(blockPos);
		}

		private void NotifyNeighboringStates(BlockPos blockPos) {
			foreach (var neighborPos in blockPos.GetCardinalNeighbors()) {
				WorldChunk? chunk = ChunkAt(blockPos);
				if (chunk == null) return;

				BlockState? blockState = GetBlockState(neighborPos);
				Block block = blockState?.block ?? Blocks.air;

				if (block is INeighborUpdateHandler neighborUpdateHandler) {
					neighborUpdateHandler.OnNeighborChanged(this, neighborPos, blockPos);
				}
			}
		}

		public void AddEntity(Entity entity) {
			Guid guid = Guid.NewGuid();
			entity.CreateTransform(guid);
			entities[guid] = entity;

			if (entity is ITickingEntity ticking) {
				tickingEntities.Add(ticking);
			}
		}

		public void RemoveEntity(Entity entity) {
			if (!entities.ContainsKey(entity.guid)) return;

			entities.Remove(entity.guid);
			entity.Dispose();

			if  (entity is ITickingEntity ticking) {
				tickingEntities.Remove(ticking);
			}
		}

		public void SpawnEntity(EntityDescriptor descriptor, Vector2 pos) {
			descriptor.Create(this, pos);
		}

		public bool TryGetEntity(Guid guid, out Entity entity) {
			return entities.TryGetValue(guid, out entity);
		}

		public IEnumerable<Entity> GetAllEntities() => entities.Values;

		public void UnloadDistantChunks(int pivotChunkX, int viewDistance) {
			List<WorldChunk> toRemove = new();

			foreach (int chunkX in loadedChunks.Keys) {
				if (Mathf.Abs(chunkX - pivotChunkX) > viewDistance) {
					toRemove.Add(loadedChunks[chunkX]);
				}
			}

			foreach (WorldChunk chunk in toRemove) {
				loadedChunks.Remove(chunk.xpos);
				chunk.OnUnload(chunkOutlineRenderer);
			}
		}

		public void OnSessionStop() {
			Soulbound.instance.GetInputManager().RemoveContext(player);
		}

		public BlockState? GetBlockState(BlockPos blockPos) {
			WorldChunk? chunk = ChunkAt(blockPos);
			return chunk?.GetBlockState(blockPos.ToChunkPos());
		}

		public TileEntity? TileEntityAt(BlockPos blockPos) {
			WorldChunk? chunk = ChunkAt(blockPos);
			return chunk?.TileEntityAt(blockPos);
		}

		public Block? GetBlock(BlockPos blockPos) {
			BlockState? blockState = GetBlockState(blockPos);
			return blockState?.block;
		}

		public static int ChunkXAt(Vector2 worldPos) => ChunkXAt(worldPos.x);
		public static int ChunkXAt(int x) => ChunkXAt((float)x);
		public static int ChunkXAt(float x) => Mathf.FloorToInt(x / CHUNK_LENGTH);

		public static int ToWorldX(int cx, int chunkX) => cx + chunkX * CHUNK_LENGTH;
		public static int ToChunkX(int x) => x - ChunkXAt(x) * CHUNK_LENGTH;

		public WorldChunk? ChunkAt(int xpos) => ChunkAt(new BlockPos(xpos, 0));
		public WorldChunk? ChunkAt(BlockPos blockPos) { 
			return generatedChunks!.GetValueOrDefault(ChunkXAt(blockPos.x), null);
		}

		public WorldChunk? GetChunk(int chunkX) {
			if (generatedChunks.TryGetValue(chunkX, out var chunk)) {
				return chunk;
			}
			return null;
		}

		public static bool IsInBounds(BlockPos pos) {
			return pos.y < WORLD_HEIGHT && pos.y >= WorldChunk.minY;
		}

		public int GetSurfaceY(int xpos) {
			int chunkX = ChunkXAt(xpos);
			int cx = ToChunkX(xpos);

			return chunkGenData.TryGetValue(chunkX, out var value)
				? value.surfacePoints[cx]
				: 0;
		}

		public int GetSurfaceAirY(int xpos) => GetSurfaceY(xpos) + 1;

		public bool IsChunkLoaded(WorldChunk chunk) => loadedChunks.ContainsValue(chunk);

		public bool IsChunkLoaded(int chunkX) => loadedChunks.ContainsKey(chunkX);

		public bool IsChunkGenerated(int chunkX) => generatedChunks.ContainsKey(chunkX);

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

		public Player GetPlayer() => player;

	}
}
