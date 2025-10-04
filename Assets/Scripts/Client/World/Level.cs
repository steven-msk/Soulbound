using SoulboundBackend.Client.World.BlockSystem;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Client.World.Entity;
using SoulboundBackend.Client.World.Structure;
using SoulboundBackend.Client.World.Structure.Templates;
using SoulboundBackend.Common;
using SoulboundBackend.Core;
using SoulboundBackend.Core.Noise;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Tilemaps;
using Application = UnityEngine.Application;
using Logger = SoulboundBackend.Common.Logging.Logger;

#nullable enable

namespace SoulboundBackend.Client.World {
    public class Level {
        private static readonly Logger logger = Logger.CreateInstance();
        public static readonly LogModule level = new LogModule("LEVEL", "#4682B4");
	    public const int CHUNK_LENGTH = 32;
        public const int WORLD_HEIGHT = 300;
        public const int SURFACE_TO_UNDERGROUND_DELIMITER = WORLD_HEIGHT / 2;
        public static string worldDumpFile => Path.Combine(Application.persistentDataPath, LevelManager.worldDump);

	    public event Action<BlockChangeInfo>? BlockStateChanged;
        // POTENTIAL: post world events to the ticking system

        public readonly int seed;
        static Dictionary<string, StructureTemplate> registeredStructureTemplates = new();
        private readonly PerlinNoiseGenerator1D heightGenerator;
        private Dictionary<int, WorldChunk> loadedChunks = new();
        private Dictionary<int, WorldChunk> generatedChunks = new(); 
        private ChunkOutlineRenderer chunkOutlineRenderer = new();
        private Dictionary<int, List<StructurePlacement>> structurePlacements = new();
        private Dictionary<int, List<(ChunkBlockPos chunkBlockPos, BlockState state)>> pendingUpdates = new();
        private int renderDistance;
        private LevelGridContext gridContext;
        private EntityManager entityManager;
        public EntityManager EntityManager => entityManager;

        private PlayerController player;
        public PlayerController Player => player;

        public bool isBootstrapped { get; private set; } = false;

        public Level(PlayerController player, LevelGridContext gridContext, int seed, int renderDistance) {
            this.player = player;
            this.gridContext = gridContext;
            this.renderDistance = renderDistance; 
            this.seed = seed;
            this.heightGenerator = new PerlinNoiseGenerator1D(this.seed, WorldChunk.HEIGHT_SPREAD);
            this.entityManager = new EntityManager(this);
        }

        // PLANNED REWORK: world rendering system
        // Since this will be an intense tiled game, it will require advanced rendering techniques to
        // achieve any level of performance. This project is still in prototype phase, so making any
        // optimization isnt really worth it and might be a waste of time in most cases.

        public void BootstrapWorld(WorldDump? dump) {
            Dictionary<int, int[][]> chunkIDmap = new();

            if (dump == null) {
			    for (int dx = -renderDistance; dx <= renderDistance; dx++) {
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

            isBootstrapped = true;

            InvocationHelper.If(dump != null, 
                () => entityManager.Boostrap(dump!.Value.serializedEntities));
            SerializedEntity fallback = new(typeof(PlayerController), 
                Guid.NewGuid(), player.prefabDefinitionID,
                new Vector2(0f, GetSurfaceY(0)), null
            );
		    entityManager.SpawnPlayer(player, dump?.player ?? fallback);
	    }

        public WorldDump CreateDump() {
            Dictionary<Guid, SerializedEntity> serializedEntities = entityManager.AllExistingEntities
                .Where(entity => entity.Value is not PlayerController)
                .ToDictionary(guid => guid.Key, entity => entity.Value.Serialize());

		    WorldDump dump = new(
                this.seed, 
                generatedChunks.Values.ToArray(), 
                player.Serialize(),
                this.structurePlacements, 
                serializedEntities
            );
            return dump;
	    }

        public void UpdateChunks(Vector2 playerPos) {
            int playerChunkX = ChunkXAt(playerPos);
            this.UnloadDistantChunks(playerChunkX, renderDistance);

            for (int dx = -renderDistance; dx <= renderDistance; dx++) {
                int chunkX = playerChunkX + dx;
                if (!loadedChunks.ContainsKey(chunkX)) {
                    WorldChunk chunk = new(chunkX);

                    if (!generatedChunks.ContainsKey(chunkX)) {
                        chunk = this.GenerateNewChunk(chunkX);
                        generatedChunks[chunkX] = chunk;

                        if (pendingUpdates.TryGetValue(chunkX, out var stateUpdates)) {
                            stateUpdates.ForEach(stateUpdate => {
                                chunk.SetBlock(stateUpdate.chunkBlockPos, stateUpdate.state);
                            });
                            pendingUpdates.Remove(chunkX);
                        }
                    } else {
                        chunk = generatedChunks[chunkX];
                    }

                    loadedChunks[chunkX] = chunk;
                    chunk.Render(gridContext.tilemap, chunkOutlineRenderer);
                    entityManager.OnChunkLoaded(chunk);
                }
            }
        }

        public void Update(float deltaTime) {
            this.UpdateChunks(player.position);
            entityManager.Update(deltaTime);
        }

        public bool IsChunkLoaded(WorldChunk chunk) => loadedChunks.ContainsValue(chunk);

        public bool IsChunkLoaded(int chunkX) => loadedChunks.ContainsKey(chunkX);

        private WorldChunk GenerateNewChunk(int chunkX) {
            WorldChunk chunk = new(chunkX);
            ChunkHeightmapData heightmapData = chunk.GenerateHeightmap(this.heightGenerator);
            generatedChunks[chunkX] = chunk;

            for (int cx = 0; cx < Level.CHUNK_LENGTH; cx++) {
                if (CheckStructureAvailability(cx, chunk.xpos, heightmapData, out var structurePlacement)) {
                    PlaceStructure(chunkX, structurePlacement);
                }
            }

            return chunk;
        }

        public bool CheckStructureAvailability(
                int chunkBlockX,
                int chunkX, 
                ChunkHeightmapData heightmapData,
                out StructurePlacement structurePlacement
            ) {
            foreach (var structureID in registeredStructureTemplates.Keys) {
                StructureTemplate structure = registeredStructureTemplates[structureID];
                var context = new StructureGenerationContext(chunkBlockX, chunkX, heightmapData, this);
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

        public StructurePlacement ForceGeneratePlacement(ChunkBlockPos chunkBlockPos, StructureTemplate template) {
            WorldChunk? chunk = chunkBlockPos.underlyingChunk;
            var context = new StructureGenerationContext(chunkBlockPos.x, chunkBlockPos.chunkX, chunk.HeightmapData, this);
            var structureData = template.placementFunction.Invoke(context, true);

            return template.FinalizePlacement(template.placementGenerator.Invoke(context, structureData));
        }

        public void ForcePlaceStructure(ChunkBlockPos chunkBlockPos, StructureTemplate template) {
            StructurePlacement placement = ForceGeneratePlacement(chunkBlockPos, template);
            PlaceStructure(chunkBlockPos.chunkX, placement);
        }

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

        public bool OverlappingStructures(BlockPos blockPos, out List<StructurePlacement> overlappingStructures) {
            int chunkX = ChunkXAt((Vector2Int)blockPos);

            if (structurePlacements.TryGetValue(chunkX, out var chunkFeatures)) {
                ChunkBlockPos chunkBlockPos = blockPos.ToChunkBlockPos(chunkX);
                overlappingStructures = chunkFeatures
                    .Where(f => f.bounds.Contains((Vector2Int)chunkBlockPos)).ToList();
                return overlappingStructures.Count > 0;
            }

            overlappingStructures = new List<StructurePlacement>();
            return false;
        }

        public void PlaceStructure(int chunkX, StructurePlacement placement) {
            if (!structurePlacements.ContainsKey(chunkX)) {
                structurePlacements.Add(chunkX, new List<StructurePlacement>());
            }
            structurePlacements[chunkX].Add(placement);

            foreach (var stateOverride in placement.stateOverrides) {
                ChunkBlockPos chunkBlockPos = stateOverride.Key;
                BlockState blockState = stateOverride.Value;
                WorldChunk? underlyingChunk = ChunkAt(chunkBlockPos.ToWorldBlockPos());

                if (underlyingChunk == null) {
                    PendUpdate(chunkBlockPos.chunkX, chunkBlockPos, blockState);
                } else if (loadedChunks.ContainsKey(chunkBlockPos.chunkX) && underlyingChunk != null) {
                    SetBlock(chunkBlockPos, blockState, updateStates: false);
                } else {
                    underlyingChunk?.SetBlock(chunkBlockPos, blockState);
                }
            }
        }

        public void MarkStructureDirty(StructurePlacement placement) { 
            structurePlacements[placement.origin.chunkX].Remove(placement);
        }

        public void SetBlock(BlockPos blockPos, BlockState? blockState, bool broadcastStateChange = true) {
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

            BlockEventType blockEventType = BlockEventType.StateMutated;
            if (newState != air && oldState == air) {
                blockEventType = BlockEventType.Placed;
            } else if (oldState != air && newState == air) {
                blockEventType = BlockEventType.Broken;
            }

            InvocationHelper.If(blockEventType == BlockEventType.Placed,
                () => newState?.OnPlace(blockPos));
            if (broadcastStateChange) {
                BlockStateChanged?.Invoke(
                    new BlockChangeInfo(blockPos, oldState, newState, this, blockEventType)
                );
            }
        }

        public void SetBlock(ChunkBlockPos chunkBlockPos, BlockState? blockState, bool updateStates = true) {
            SetBlock(chunkBlockPos.ToWorldBlockPos(), blockState, updateStates);
        }

        // in the future this will also contain the information about how the block was broken
        public void BreakBlock(BlockPos blockPos, BreakSource provider) {
            BlockState? brokenBlock = BlockStateAt(blockPos);
            SetBlock(blockPos, null);
            brokenBlock?.DropOnBroken(blockPos, provider);
        }

        public void PendUpdates(int chunkX, List<(ChunkBlockPos chunkBlockpos, BlockState state)> blockStateUpdates) {
            if (pendingUpdates.TryGetValue(chunkX, out var existingUpdates)) {
                existingUpdates.AddRange(blockStateUpdates);
            } else {
                pendingUpdates.Add(chunkX, blockStateUpdates);
            }
        }

        public void PendUpdate(int chunkX, ChunkBlockPos chunkBlockPos, BlockState blockState) {
            if (!pendingUpdates.ContainsKey(chunkX)) {
                pendingUpdates[chunkX] = new List<(ChunkBlockPos, BlockState)>();
            }
            pendingUpdates[chunkX].Add((chunkBlockPos, blockState));
        }

        public void UnloadDistantChunks(int playerChunkX, int viewDistance) {
            List<WorldChunk> toRemove = new();

            foreach (int chunkX in loadedChunks.Keys) {
                if (Mathf.Abs(chunkX - playerChunkX) > viewDistance) {
                    toRemove.Add(loadedChunks[chunkX]);
                }
            }

            foreach (WorldChunk chunk in toRemove) {
                loadedChunks.Remove(chunk.xpos);
                chunk.Unload(gridContext.tilemap, chunkOutlineRenderer);
                entityManager.OnChunkUnloaded(chunk);
            }
        }

        public BlockState? BlockStateAt(BlockPos blockPos, bool logFlag = true) {
            WorldChunk? chunk = ChunkAt(blockPos);

            if (chunk != null) {
                return chunk.BlockStateAt(blockPos.ToChunkBlockPos(chunk.xpos));
            }

            InvocationHelper.If(logFlag,
                () => logger.LogError(level, $"Cannot retrieve block state at {blockPos.ToString()} because its not generated"));
            return null;
        }

        public Block? BlockAt(BlockPos blockPos) {
            BlockState? blockState = BlockStateAt(blockPos, logFlag: false);
            if (blockState != null) {
                return blockState.block;
            }

		    logger.LogError(level, $"Cannot retrieve block at {blockPos.ToString()} because its not generated");
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

        public ChunkBlockPos ToChunkPos(Vector2 worldPos) {
            BlockPos blockPos = ToBlockPos(worldPos);
            return ChunkBlockPos.FromBlockPos(blockPos);
        }

        public int ToWorldX(int chunkBlockX, int chunkX) => chunkBlockX + chunkX * CHUNK_LENGTH;

        public int ToChunkX(int worldX) => worldX - ChunkXAt(worldX) * CHUNK_LENGTH;

        public int GetSurfaceY(BlockPos blockPos) => GetSurfaceY(blockPos.x);

        public int GetSurfaceY(int xpos) => this.ChunkAt(xpos)?.HeightmapData.surfaceLevels[xpos] ?? 0;

        public bool IsInWorldBounds(Vector2 pos) => pos.y >= WorldChunk.minY && pos.y <= WorldChunk.maxY;

        public bool IsInWorldBounds(BlockPos blockPos) => IsInWorldBounds((Vector2)blockPos);

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

        private static void RegisterStructure(StructureTemplate structure) {
            registeredStructureTemplates.Add(structure.ID, structure);
        }

        static Level() {
            RegisterStructure(TreeStructure.instance);
        }
    }
}