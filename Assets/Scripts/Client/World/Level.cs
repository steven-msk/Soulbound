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
using Application = UnityEngine.Application;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.World {
	public delegate void OnChunkGenerated(ChunkGenData genData);

	public sealed class Level : ITickable {
		private static readonly Logger logger = Logger.CreateInstance();
		public static readonly LogModule level = new LogModule("LEVEL", "#4682B4");
		public const int CHUNK_LENGTH = 32;
		public const int WORLD_HEIGHT = 1024;
		[Obsolete] public const int SURFACE_TO_UNDERGROUND_DELIMITER = WORLD_HEIGHT / 2;
		public const int RENDER_DISTANCE = 8;
		public const int TERRAIN_PLANE_Y = 0;
		const int biomeBlendRange = 10;
		[Obsolete] public static string worldDumpFile => Path.Combine(Application.persistentDataPath, LevelManager.worldDump);

		public event Action<BlockChangeInfo>? BlockStateChanged;
		// POTENTIAL: post world events to the ticking system

		public readonly int seed;
		[Obsolete] static Dictionary<string, StructureTemplate> registeredStructureTemplates = new();
		[Obsolete] private readonly PerlinNoiseGenerator1D heightGenerator;
		private Dictionary<int, WorldChunk> loadedChunks = new();
		private Dictionary<int, WorldChunk> generatedChunks = new(); 
		private ChunkOutlineRenderer chunkOutlineRenderer = new();
		[Obsolete] private Dictionary<int, List<StructurePlacement>> structurePlacements = new();
		[Obsolete] private Dictionary<int, List<(ChunkBlockPos pos, BlockState? state)>> pendingUpdates = new();
		[Obsolete] private readonly ConcurrentDictionary<int, List<OnChunkGenerated>> deferredGenerations = new();
		private readonly Dictionary<int, ChunkGenData> chunkGenData = new();
		private LevelGridContext gridContext;
		private LevelManager levelManager = null!;
		public bool isWorldLoaded { get; private set; } = false;

		private readonly BiomeMap biomeMap;
		private readonly Heightmap heightmap;
		private readonly Cavemap cavemap;

		public Level(LevelGridContext gridContext, int seed) {
			this.gridContext = gridContext;
			this.seed = seed;
			this.heightGenerator = new PerlinNoiseGenerator1D(this.seed, WorldChunk.HEIGHT_SPREAD);

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
				for (int dx = -RENDER_DISTANCE; dx <= RENDER_DISTANCE; dx++) {
					this.GenerateNewChunk(dx);
				}
			} else {
				this.generatedChunks = dump.Value.generatedChunks.ToDictionary(chunk => chunk.xpos, chunk => chunk);
				this.structurePlacements = dump.Value.structurePlacements;
			}

			foreach (var structureTemplate in registeredStructureTemplates.Values) {
				if (structureTemplate?.blockStateChangedCallback is not null) {
					BlockStateChanged += structureTemplate.blockStateChangedCallback;
				}
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
			return new Vector2(0f, GetSurfaceY(0));
		}

		public void Dump(out int seed, out WorldChunk[] generatedChunks, out Dictionary<int, List<StructurePlacement>> structurePlacements) {
			seed = this.seed;
			generatedChunks = this.generatedChunks.Values.ToArray();
			structurePlacements = this.structurePlacements;
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
					chunk.Render(gridContext.tilemap, chunkOutlineRenderer);
					levelManager.OnChunkLoaded(chunk);
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
			ChunkHeightmapData heightmapData = chunk.GenerateHeightmap(this.heightGenerator);

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

		[Obsolete]
		public bool TryPlaceStructure(
				int chunkBlockX,
				int chunkX, 
				ChunkHeightmapData heightmapData,
				out StructurePlacement structurePlacement
			) {
			foreach (var structureID in registeredStructureTemplates.Keys) {
				StructureTemplate structure = registeredStructureTemplates[structureID];
				var context = new StructureGenerationContext(chunkBlockX, 0, chunkX, heightmapData, this);
				var data = structure.placementFunction.Invoke(context, false);

				if (data != null && structure.validationFunction.Invoke(context, data.Value)) {
					var placementConstraints = structure.placementGenerator.Invoke(context, data.Value);
					structurePlacement = structure.FinalizePlacement(placementConstraints);
					return true;
				}
			}

			structurePlacement = null!;
			return false;
		}

		[Obsolete]
		public StructurePlacement ForceGeneratePlacement(ChunkBlockPos chunkBlockPos, StructureTemplate template) {
			WorldChunk? chunk = chunkBlockPos.UnderlyingChunk(this);
			var context = new StructureGenerationContext(chunkBlockPos.x, chunkBlockPos.y, chunkBlockPos.chunkX, chunk.HeightmapData, this);
			var structureData = template.placementFunction.Invoke(context, true);

			return template.FinalizePlacement(template.placementGenerator.Invoke(context, structureData));
		}

		[Obsolete]
		public void ForcePlaceStructure(ChunkBlockPos chunkBlockPos, StructureTemplate template) {
			StructurePlacement placement = ForceGeneratePlacement(chunkBlockPos, template);
			PlaceStructure(chunkBlockPos.chunkX, placement);
		}

		[Obsolete]
		public bool StructureAt(BlockPos blockPos, out StructurePlacement structure) {
			int chunkX = ChunkXAt((Vector2Int)blockPos);
			if (structurePlacements.TryGetValue(chunkX, out var chunkFeatures)) {
				ChunkBlockPos chunkBlockPos = blockPos.ToChunkBlockPos(chunkX);
				structure = chunkFeatures
					.FirstOrDefault(f => f.bounds.Contains((Vector2Int)chunkBlockPos));
				return structure?.PersistentExistence() ?? false;
			}

			structure = null!;
			return false;
		}


		[Obsolete]
		public bool OverlappingStructures(BlockPos blockPos, out List<StructurePlacement> overlappingStructures) {
			int chunkX = ChunkXAt((Vector2Int)blockPos);

			if (structurePlacements.TryGetValue(chunkX, out var placementsInChunk)) {
				ChunkBlockPos chunkBlockPos = blockPos.ToChunkBlockPos(chunkX);
				overlappingStructures = placementsInChunk
					.Where(f => f.bounds.Contains((Vector2Int)chunkBlockPos))
					.ToList();
				return overlappingStructures.Count > 0;
			}

			overlappingStructures = new List<StructurePlacement>();
			return false;
		}

		[Obsolete]
		public void PlaceStructure(int chunkX, StructurePlacement placement) {
			if (!structurePlacements.ContainsKey(chunkX)) {
				structurePlacements.Add(chunkX, new List<StructurePlacement>());
			}
			structurePlacements[chunkX].Add(placement);

			foreach (var stateOverride in placement.stateOverrides) {
				ChunkBlockPos chunkBlockPos = stateOverride.Key;
				BlockState blockState = stateOverride.Value;
				WorldChunk? underlyingChunk = chunkBlockPos.UnderlyingChunk(this);

				if (underlyingChunk == null) {
					PendBlock(chunkBlockPos, blockState);
				} else if (loadedChunks.ContainsKey(chunkBlockPos.chunkX) && underlyingChunk != null) {
					SetBlock(chunkBlockPos, blockState);
				} else {
					underlyingChunk?.SetBlock(chunkBlockPos, blockState);
				}
			}
		}


		[Obsolete]
		public void MarkStructureDirty(StructurePlacement placement) { 
			structurePlacements[placement.origin.chunkX].Remove(placement);
		}

		public void SetBlock(BlockPos blockPos, BlockState? blockState) {
			WorldChunk? chunk = this.ChunkAt(blockPos);
			BlockState air = Blocks.air.defaultState;
			BlockState oldState = chunk?.BlockStateAt(blockPos.ToChunkBlockPos(chunk.xpos)) ?? air;
			BlockState newState = blockState ?? air;
			TileBase referencedTile = newState.block.tileReference;
			if (oldState == newState) {
				return;
			}

			chunk?.SetBlock(blockPos.ToChunkBlockPos(chunk.xpos), newState);
			gridContext.tilemap.SetTile((Vector3Int)blockPos, referencedTile);
			blockPos.ForEachAdjacent((direction, neighborPos) => {
				BlockState? neighborBlockState = BlockStateAt(neighborPos);
				neighborBlockState?.OnNeighborStateChanged(neighborPos, blockPos, oldState, newState);
				gridContext.tilemap.RefreshTile((Vector3Int)neighborPos);
			});
		}

		public void SetBlock(ChunkBlockPos chunkBlockPos, BlockState? blockState) {
			SetBlock(chunkBlockPos.ToBlockPos(), blockState);
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
			BlockState? oldState = BlockStateAt(blockPos);
			SetBlock(blockPos, newState);
			BroadcastBlockEvent(new BlockChangeInfo(
				blockPos,
				oldState,
				BlockStateAt(blockPos),
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
			BlockState? brokenBlock = BlockStateAt(blockPos);
			SetBlock(blockPos, null);
			BroadcastBlockEvent(new BlockChangeInfo(
				blockPos, 
				brokenBlock,
				BlockStateAt(blockPos),
				this,
				BlockEventType.Broken,
				Optional<BreakSource>.Of(source)
			), (oldState, newState) => oldState?.DropOnBroken(blockPos, source));
		}

		[Obsolete]
		public void PendBlocks(int chunkX, List<(ChunkBlockPos chunkBlockpos, BlockState? state)> blockStateUpdates) {
			if (pendingUpdates.TryGetValue(chunkX, out var existingUpdates)) {
				existingUpdates.AddRange(blockStateUpdates);
			} else {
				pendingUpdates.Add(chunkX, blockStateUpdates);
			}
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
				chunk.Unload(gridContext.tilemap, chunkOutlineRenderer);
				levelManager.OnChunkUnloaded(chunk);
			}
		}

		public BlockState? BlockStateAt(BlockPos blockPos, bool logFlag = true) {
			WorldChunk? chunk = ChunkAt(blockPos);

			if (chunk != null) {
				return chunk.BlockStateAt(blockPos.ToChunkBlockPos(chunk.xpos));
			}

			//InvocationHelper.If(logFlag,
			//	() => logger.LogError(level, $"Cannot retrieve block state at {blockPos.ToString()} because its not generated"));
			return null;
		}

		public Block? BlockAt(BlockPos blockPos) {
			BlockState? blockState = BlockStateAt(blockPos, logFlag: false);
			if (blockState != null) {
				return blockState.block;
			}

			//logger.LogError(level, $"Cannot retrieve block at {blockPos.ToString()} because its not generated");
			return null;
		}

		public BlockState? GetAdjacentBlockState(BlockPos startPos, Direction direction) {
			BlockPos adjacentPos = startPos + direction.AsVector();
			return BlockStateAt(adjacentPos);
		}

		public int ChunkXAt(Vector2 worldPos) => Mathf.FloorToInt(worldPos.x / CHUNK_LENGTH);

		public int ChunkXAt(float x) => Mathf.FloorToInt(x / CHUNK_LENGTH);

		public WorldChunk? ChunkAt(BlockPos blockPos) { 
			return generatedChunks!.GetValueOrDefault(this.ChunkXAt((Vector2)blockPos), null);
		}

		public WorldChunk? ChunkAt(int xpos) => ChunkAt(new BlockPos(xpos, 0));

		public BlockPos ToBlockPos(Vector2 worldPos) {
			Vector2Int intPos = (Vector2Int)gridContext.grid.WorldToCell(worldPos);
			return new BlockPos(intPos.x, intPos.y);
		}

		public WorldChunk? ToChunk(int chunkX) {
			if (generatedChunks.TryGetValue(chunkX, out var chunk)) {
				return chunk;
			}
			return null;
		}

		public ChunkBlockPos ToChunkPos(Vector2 worldPos) {
			BlockPos blockPos = ToBlockPos(worldPos);
			return ChunkBlockPos.FromBlockPos(blockPos);
		}

		public int ToWorldX(int chunkBlockX, int chunkX) => chunkBlockX + chunkX * CHUNK_LENGTH;

		public int ToChunkX(int worldX) => worldX - ChunkXAt(worldX) * CHUNK_LENGTH;

		public int NormalizeChunkX(int x) {
			return (x % CHUNK_LENGTH + CHUNK_LENGTH) % CHUNK_LENGTH;
		}

		public int ChunkXFromRelativeBlock(int chunkBlockX, int chunkX) {
			int worldX = ToWorldX(chunkBlockX, chunkX);
			return FloorDiv(worldX, CHUNK_LENGTH);
		}

		public int GetSurfaceY(BlockPos blockPos) => GetSurfaceY(blockPos.x);

		public int GetSurfaceY(int xpos) => this.ChunkAt(xpos)?.HeightmapData.surfaceLevels[xpos] ?? 0;

		public bool IsInWorldBounds(Vector2 pos) => pos.y >= WorldChunk.minY && pos.y <= WorldChunk.maxY;

		public bool IsInWorldBounds(BlockPos blockPos) => IsInWorldBounds((Vector2)blockPos);

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

		public static void RegisterStructure(StructureTemplate structure) {
			if (!registeredStructureTemplates.ContainsKey(structure.ID)) {
				registeredStructureTemplates.Add(structure.ID, structure);
			}
		}

		public static void ClearStructureRegistry() {
			registeredStructureTemplates.Clear();
		}

		static int FloorDiv(int a, int b) {
			int q = a / b;
			int r = a % b;

			if (r != 0 && ((r < 0) != (b < 0))) {
				q--;
			}
			return q;
		}
	}
}