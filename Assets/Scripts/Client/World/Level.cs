using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static Unity.Collections.AllocatorManager;
using Random = UnityEngine.Random;

#nullable enable

public class Level {
    public const int CHUNK_LENGTH = 32;
    public const int WORLD_HEIGHT = 300;
    public const int SURFACE_TO_UNDERGROUND_DELIMITER = WORLD_HEIGHT / 2;

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
    private EntityManager entityManager;
    public EntityManager EntityManager => entityManager;

    private Grid grid;

    private PlayerController player;
    public PlayerController Player => player;

    private Tilemap tilemap;
    public Tilemap WorldTilemap => tilemap;

    public Level(PlayerController player, Tilemap tilemap, Grid grid, int seed, int renderDistance) {
        this.player = player;
        this.tilemap = tilemap;
        this.grid = grid;
        this.renderDistance = renderDistance; 
        this.seed = seed;
        this.heightGenerator = new PerlinNoiseGenerator1D(this.seed, WorldChunk.HEIGHT_SPREAD);
        this.entityManager = new EntityManager(this);
    }

    // PLANNED REWORK: world rendering system
    // Since this will be an intense tiled game, it will require advanced rendering techniques to
    // achieve any level of performance. This project is still in prototype phase, so making any
    // optimization isnt really worth it and might be a waste of time in most cases.

    public void BootstrapWorld(Vector2 lastPlayerPos) {
        int playerChunkX = ChunkXAt(lastPlayerPos);
        for (int dx = -renderDistance; dx <= renderDistance; dx++) {
            int chunkX = playerChunkX + dx;
            this.GenerateNewChunk(chunkX);
        }
        foreach (var structureTemplate in registeredStructureTemplates.Values) {
            if (structureTemplate?.blockStateChangedCallback is not null) {
                BlockStateChanged += structureTemplate.blockStateChangedCallback;
            }
        }
        Vector2 spawnPos = new(0f, GetSurfaceY(0));
        entityManager.SpawnPlayer(player, new EntitySpawnData(spawnPos));
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
                        stateUpdates.ForEach(stateUpdate => chunk.SetBlock(stateUpdate.chunkBlockPos, stateUpdate.state));
                        pendingUpdates.Remove(chunkX);
                    }
                } else {
                    chunk = generatedChunks[chunkX];
                }
                loadedChunks[chunkX] = chunk;
                chunk.Render(tilemap, chunkOutlineRenderer);
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

    public bool CheckStructureAvailability(int chunkBlockX, int chunkX, ChunkHeightmapData heightmapData, out StructurePlacement structurePlacement) {
        foreach (var structureID in registeredStructureTemplates.Keys) {
            StructureTemplate structure = registeredStructureTemplates[structureID];
            StructureGenerationContext context = new StructureGenerationContext(chunkBlockX, chunkX, heightmapData, this);
            PreliminaryStructureData? data = structure.placementFunction.Invoke(context, false);
            if (data != null && structure.validationFunction.Invoke(context, data.Value)) {
                StructurePlacementConstraints placementConstraints = structure.placementGenerator.Invoke(context, data.Value);
                structurePlacement = structure.FinalizePlacement(placementConstraints);
                return true;
            }
        }
        structurePlacement = null!;
        return false;
    }

    public StructurePlacement ForceGeneratePlacement(ChunkBlockPos chunkBlockPos, StructureTemplate template) {
        WorldChunk? chunk = chunkBlockPos.underlyingChunk;
        StructureGenerationContext context = new(chunkBlockPos.x, chunkBlockPos.chunkX, chunk.HeightmapData, this);
        PreliminaryStructureData? structureData = template.placementFunction.Invoke(context, true);
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
            structure = chunkFeatures.FirstOrDefault(f => f.bounds.Contains((Vector2Int)chunkBlockPos));
            return structure?.PersistentExistence() ?? false;
        }
        structure = null!;
        return false;
    }

    public bool OverlappingStructures(BlockPos blockPos, out List<StructurePlacement> overlappingStructures) {
        int chunkX = ChunkXAt((Vector2Int)blockPos);
        if (structurePlacements.TryGetValue(chunkX, out var chunkFeatures)) {
            ChunkBlockPos chunkBlockPos = blockPos.ToChunkBlockPos(chunkX);
            overlappingStructures = chunkFeatures.Where(f => f.bounds.Contains((Vector2Int)chunkBlockPos)).ToList();
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

    public void MarkStructureDirty(StructurePlacement placement) => structurePlacements[placement.origin.chunkX].Remove(placement);

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
        tilemap.SetTile((Vector3Int)blockPos, referencedTile);
        blockPos.ForEachAdjacent((direction, neighborPos) => {
            BlockState? neighborBlockState = BlockStateAt(neighborPos);
            neighborBlockState?.OnNeighborStateChanged(neighborPos, blockPos, oldState, newState);
            tilemap.RefreshTile((Vector3Int)neighborPos);
        });
        BlockEventType blockEventType = BlockEventType.StateMutated;
        if (newState != air && oldState == air) {
            blockEventType = BlockEventType.Placed;
        } else if (oldState != air && newState == air) {
            blockEventType = BlockEventType.Broken;
        }
        InvocationHelper.If(blockEventType == BlockEventType.Placed, () => newState?.OnPlace(blockPos));
        if (broadcastStateChange) {
            BlockStateChanged?.Invoke(new BlockChangeInfo(blockPos, oldState, newState, this, blockEventType));
        }
    }

    public void SetBlock(ChunkBlockPos chunkBlockPos, BlockState? blockState, bool updateStates = true) {
        SetBlock(chunkBlockPos.ToWorldBlockPos(), blockState, updateStates);
    }

    // in the future this will also contain the information about how the block was broken
    public void BreakBlock(BlockPos blockPos, BreakSource source) {
        BlockState? brokenBlock = BlockStateAt(blockPos);
        SetBlock(blockPos, null);
        brokenBlock?.DropOnBroken(blockPos, source);
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
            chunk.Unload(tilemap, chunkOutlineRenderer);
            entityManager.OnChunkUnloaded(chunk);
        }
    }

    public BlockState? BlockStateAt(BlockPos blockPos, bool logFlag = true) {
        WorldChunk? chunk = ChunkAt(blockPos);
        if (chunk != null) {
            return chunk.BlockStateAt(blockPos.ToChunkBlockPos(chunk.xpos));
        }
        logFlag.If(() => UnityEngine.Debug.LogError($"Cannot retrieve block state at {blockPos.ToString()} because its not generated"));
        return null;
    }

    public Block? BlockAt(BlockPos blockPos) {
        BlockState? blockState = BlockStateAt(blockPos, logFlag: false);
        if (blockState != null) {
            return blockState.block;
        }
		UnityEngine.Debug.LogError($"Cannot retrieve block at {blockPos.ToString()} because its not generated");
        return null;
    }

    public BlockState? GetAdjacentBlockState(BlockPos startPos, Direction direction) {
        BlockPos adjacentPos = startPos + direction.AsVector();
        return BlockStateAt(adjacentPos);
    }

    public int ChunkXAt(Vector2 worldPos) => Mathf.FloorToInt(worldPos.x / CHUNK_LENGTH);

    public int ChunkXAt(float x) => Mathf.FloorToInt(x / CHUNK_LENGTH);

    public WorldChunk? ChunkAt(BlockPos blockPos) => generatedChunks!.GetValueOrDefault(this.ChunkXAt((Vector2)blockPos), null);

    public WorldChunk? ChunkAt(int xpos) => ChunkAt(new BlockPos(xpos, 0));

    public BlockPos ToBlockPos(Vector2 worldPos) {
        Vector2Int intPos = (Vector2Int)grid.WorldToCell(worldPos);
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
        Vector2Int min = (Vector2Int)grid.WorldToCell(bounds.min);
        Vector2Int max = (Vector2Int)grid.WorldToCell(bounds.max);
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