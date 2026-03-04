using Assets.Scripts.Client.World.Biome;
using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.EntitySystem;
using SoulboundBackend.Client.World.Generation;
using SoulboundBackend.Client.World.Structure;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Noise;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World {
	public delegate void OnChunkGenerated(ChunkGenData genData);

	public sealed class Level : ITickable, ILevelExecutionService {
		public const int CHUNK_LENGTH = 32;
		public const int WORLD_HEIGHT = 1024;
		public const int RENDER_DISTANCE = 8;
		public const int TERRAIN_PLANE_Y = 0;
		const int biomeBlendRange = 10;

		public event Action<BlockChangeInfo>? BlockStateChanged;
		// POTENTIAL: post world events to the ticking system

		public readonly int seed;
		private readonly Dictionary<int, WorldChunk> loadedChunks = new();
		private Dictionary<int, WorldChunk> generatedChunks = new(); 
		private readonly ChunkOutlineRenderer chunkOutlineRenderer = new();
		[Obsolete] private Dictionary<int, List<(ChunkBlockPos pos, BlockState? state)>> pendingUpdates = new();
		[Obsolete] private readonly ConcurrentDictionary<int, List<OnChunkGenerated>> deferredGenerations = new();
		private readonly Dictionary<int, ChunkGenData> chunkGenData = new();
		private readonly LevelGridContext gridContext;
		private LevelManager levelManager = null!;
		public bool isWorldLoaded { get; private set; } = false;

		private readonly BiomeMap biomeMap;
		private readonly Heightmap heightmap;
		private readonly Cavemap cavemap;

		public Level(LevelGridContext gridContext, int seed) {
			this.gridContext = gridContext;
			this.seed = seed;

			var biome1 = new PlainsBiome_test(seed);
			var biome2 = new HillsBiome_test(seed);
			this.biomeMap = new BiomeMap(new IBiome[] { biome1, biome2 });
			this.heightmap = new Heightmap(TERRAIN_PLANE_Y);
			this.cavemap = new Cavemap(seed);
		}

		// PLANNED REWORK: world rendering system
		// Since this will be an intense tiled game, it will require advanced rendering techniques to
		// achieve any level of performance. This project is still in prototype phase, so making any
		// optimization isnt really worth it and might be a waste of time in most cases.

		public void BootstrapWorld(WorldDump? dump, LevelManager levelManager) {
			this.levelManager = levelManager;
			Dictionary<int, int[][]> chunkIDmap = new();

			if (dump == null) {
				for (int cx = -RENDER_DISTANCE; cx <= RENDER_DISTANCE; cx++) {
					GenerateNewChunk(cx);
				}
			} else {
				generatedChunks = dump.Value.generatedChunks.ToDictionary(chunk => chunk.xpos, chunk => chunk);
			}

			Vector2 spawnPoint = dump != null
				? dump.Value.player.lastPosition
				: GetWorldSpawnPoint();
			this.UpdateChunks(spawnPoint);

			isWorldLoaded = true;
		}

		public void Tick() {
			foreach (var chunk in loadedChunks.Values) {
				chunk.Tick();
			}
		}

		public Vector2 GetWorldSpawnPoint() {
			return new Vector2(0f, GetSurfaceAirY(0));
		}

		public void Dump(out int seed, out WorldChunk[] generatedChunks) {
			seed = this.seed;
			generatedChunks = this.generatedChunks.Values.ToArray();
		}

		public void UpdateChunks(Vector2 pivot) {
			int pivotChunkX = ChunkXAt(pivot);
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

					if (pendingUpdates.TryGetValue(chunkX, out var stateUpdates)) {
						stateUpdates.ForEach(stateUpdate => {
							chunk.SetBlock(stateUpdate.pos, stateUpdate.state);
						});
						pendingUpdates.Remove(chunkX);
					}

					loadedChunks[chunkX] = chunk;
					chunk.OnLoad(chunkOutlineRenderer);
				}
			}
		}
		
		// currently world generation uses WorldChunk as the logic executor
		// when in reality it shouldnt at all
		// the target design will use world "macroing"
		// start with small artifacts, then expand and apply the same rules
		// eventually, after enough steps, the result would be the world itself
		// its only a matter of how these steps are defined as
		// since this is a far too complex feature, it will be delayed 
		// until further notice on world generation
		// for now all implementations regarding this are obsolete
		
		private WorldChunk GenerateNewChunk(int chunkX) {
			WorldChunk chunk = new(chunkX);
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
			WorldChunk chunk = ChunkAt(blockPos)
				?? throw new InvalidOperationException("Block pos not valid: " + blockPos);
			chunk.SetBlockState(blockPos.ToChunkPos(), blockState);
			levelManager.RenderRequest(blockPos, blockState);
		}

		[PROTOTYPICAL]
		public void InteractBlock(BlockPos blockPos) {
			// provisory

			BlockState? blockState = GetBlockState(blockPos);
			Block block = blockState?.block ?? Blocks.air;

			if (block is IBlockInteractionHandler interactionHandler) {
				interactionHandler.OnInteract(this, blockPos, blockState);
			}
		}



		public void SetBlock(BlockPos blockPos, BlockState? blockState) {
			WorldChunk? chunk = this.ChunkAt(blockPos);
			blockState ??= Blocks.air.defaultState;
			BlockState? oldState = chunk?.BlockStateAt(blockPos.ToChunkPos()) ?? null;
			if (oldState == blockState) {
				return;
			}

			chunk?.SetBlock(blockPos.ToChunkPos(), blockState);
			levelManager.RenderRequest(blockPos, blockState);
			blockPos.ForEachAdjacent((direction, neighborPos) => {
				BlockState? neighborBlockState = GetBlockState(neighborPos);

				// as of the BlockState refactor, block logic shouldnt live in BlockState
				//neighborBlockState?.OnNeighborStateChanged(neighborPos, blockPos, oldState, blockState);

				gridContext.tilemap.RefreshTile((Vector3Int)neighborPos);
			});
		}

		public void SetBlock(ChunkBlockPos chunkBlockPos, BlockState? blockState) {
			SetBlock(chunkBlockPos.ToBlock(), blockState);
		}

		public void SetBlockOrPend(ChunkBlockPos chunkBlockPos, BlockState? blockState) {
			int chunkX = chunkBlockPos.chunkX;

			if (generatedChunks.ContainsKey(chunkX)) {
				SetBlock(chunkBlockPos, blockState);
			} else {
				PendBlock(chunkBlockPos, blockState);
			}
		}

		public void PlaceBlock(BlockPos blockPos, BlockState newState) {
			BlockState? oldState = GetBlockState(blockPos);
			SetBlock(blockPos, newState);
			BroadcastBlockEvent(new BlockChangeInfo(
				blockPos,
				oldState,
				GetBlockState(blockPos),
				this,
				BlockEventType.Placed,
				Optional<BreakSource>.Empty()
			));
		}

		public void BroadcastBlockEvent(BlockChangeInfo changeInfo, Action<BlockState?, BlockState?>? followUp = null) {
			followUp?.Invoke(changeInfo.oldState, changeInfo.newState);
			BlockStateChanged?.Invoke(changeInfo);
		}

		public void BreakBlock(BlockPos blockPos, BreakSource source) {
			BlockState? brokenBlock = GetBlockState(blockPos);
			SetBlock(blockPos, null);

			// as of the BlockState refactor, block logic shouldnt live in BlockState

			//BroadcastBlockEvent(new BlockChangeInfo(
			//	blockPos, 
			//	brokenBlock,
			//	BlockStateAt(blockPos),
			//	this,
			//	BlockEventType.Broken,
			//	Optional<BreakSource>.Of(source)
			//), (oldState, newState) => oldState?.DropOnBroken(blockPos, source));
		}

		[Obsolete]
		public void PendBlock(ChunkBlockPos chunkBlockPos, BlockState? blockState) {
			int chunkX = chunkBlockPos.chunkX;

			if (!pendingUpdates.ContainsKey(chunkX)) {
				pendingUpdates[chunkX] = new List<(ChunkBlockPos, BlockState?)>();
			}
			pendingUpdates[chunkX].Add((chunkBlockPos, blockState));
		}

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

		public BlockState? GetBlockState(BlockPos blockPos) {
			WorldChunk? chunk = ChunkAt(blockPos);
			return chunk?.BlockStateAt(blockPos.ToChunkPos());
		}

		public TileEntity? TileEntityAt(BlockPos blockPos) {
			WorldChunk? chunk = ChunkAt(blockPos);
			return chunk?.TileEntityAt(blockPos.ToChunkPos());
		}

		public Block? BlockAt(BlockPos blockPos) {
			BlockState? blockState = GetBlockState(blockPos);
			return blockState?.block;
		}

		public BlockState? GetAdjacentBlockState(BlockPos startPos, Direction direction) {
			BlockPos adjacentPos = startPos + direction.AsVector();
			return GetBlockState(adjacentPos);
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
			Vector2Int min = (Vector2Int)gridContext.grid.WorldToCell(bounds.min);
			Vector2Int max = (Vector2Int)gridContext.grid.WorldToCell(bounds.max);

			for (int x = min.x; x <= max.x; x++) {
				for (int y = min.y; y <= max.y; y++) {
					coveredTiles.Add(new BlockPos(x, y));
				}
			}
			return coveredTiles;
		}
	}
}
