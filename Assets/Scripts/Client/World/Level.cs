using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static Unity.Collections.AllocatorManager;
using Random = UnityEngine.Random;

public class Level {
	public const int CHUNK_LENGTH = 32;
	public const int WORLD_HEIGHT = 300;
	public const int SURFACE_TO_UNDERGROUND_DELIMITER = WORLD_HEIGHT / 2;

	public readonly int seed;
	private readonly PerlinNoiseGenerator1D heightGenerator;
	private Dictionary<int, WorldChunk> loadedChunks = new();
	private Dictionary<int, WorldChunk> generatedChunks = new();
	private ChunkOutlineRenderer chunkOutlineRenderer = new();
	private Dictionary<int, List<TerrainFeature>> features = new();
	private Dictionary<int, List<(ChunkBlockPos chunkBlockPos, BlockState state)>> pendingUpdates = new();
	private int renderDistance;

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
	}

	// PLANNED REWORK: world rendering system
	// Since this will be an intense tiled game, it will require advanced rendering techniques to
	// achieve any level of performance. This project is still in prototype phase, so making any
	// optimization isnt really worth it and might be a waste of time in most cases.

	public void EarlyGenerateChunks(Vector2 playerPos) {
		int playerChunkX = ChunkXAt(playerPos);
		for (int dx = -renderDistance; dx <= renderDistance; dx++) {
			int chunkX = playerChunkX + dx;
			this.GenerateNewChunk(chunkX);
		}
	}

	private void prototype_GenerateTreeFeatures(WorldChunk chunk, ChunkHeightmapData heightmapData) {
		float treeFrequency = 0.5f;
		float treeDisparity = 0.5f;
		int chunkX = chunk.xpos;

		for (int cx = 0; cx < Level.CHUNK_LENGTH; cx++) {
			float treeNoise = Mathf.PerlinNoise(cx * treeFrequency, seed);
			if (treeNoise > treeDisparity) {
				TerrainFeature treeFeature = GenerateTerrainFeature(TerrainFeatureType.Tree, cx, chunkX, heightmapData);
				ChunkBlockPos featureOrigin = new(treeFeature.origin.x, treeFeature.origin.y, chunkX);
				if (!features.ContainsKey(chunkX)) {
					features.Add(chunkX, new List<TerrainFeature>());
				}
				features[chunkX].Add(treeFeature);
			}
		}
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
			}
		}
    }

	private WorldChunk GenerateNewChunk(int chunkX) {
		WorldChunk chunk = new(chunkX);
		ChunkHeightmapData heightmapData = chunk.GenerateHeightmap(this.heightGenerator);
		generatedChunks[chunkX] = chunk;
		this.prototype_GenerateTreeFeatures(chunk, heightmapData);

		foreach (var chunkFeatures in features.Values) {
			List<TerrainFeature> newFeatures = chunkFeatures.Where(feature => feature.origin.chunkX == chunkX).ToList();
			newFeatures.ForEach(feature => {
				foreach (var stateOverride in feature.stateOverrides) {
					ChunkBlockPos chunkBlockPos = stateOverride.Key;
					BlockState blockState = stateOverride.Value;
					WorldChunk chunk = ChunkAt(chunkBlockPos.ToWorldBlockPos());
					if (chunk == null) {
						PendUpdate(chunkBlockPos.chunkX, chunkBlockPos, blockState);
					} else if (loadedChunks.ContainsKey(chunkBlockPos.chunkX) && chunk != null) {
						SetBlockAndUpdate(chunkBlockPos, blockState);
					} else {
						chunk.SetBlock(chunkBlockPos, blockState);
					}
				}
			});
		}
		return chunk;
	}

	public TerrainFeature GenerateTerrainFeature(TerrainFeatureType type, int chunkBlockX, int chunkX, ChunkHeightmapData heightmapData) {
		switch (type) {
			case TerrainFeatureType.Tree: {
				int minHeight = 5;
				int maxHeight = 20;
				int crownRadius = 2;
				BlockState woodTile = new BlockState(Blocks.wood);
                BlockState leafTile = new BlockState(Blocks.leaf);

				ChunkBlockPos treeOrigin = new(chunkBlockX, heightmapData.surfaceLevels[ToWorldX(chunkBlockX, chunkX)], chunkX);
				ChunkBlockPos trunkPos = new ChunkBlockPos(treeOrigin.x, treeOrigin.y, chunkX);
				Dictionary<ChunkBlockPos, BlockState> stateOverrides = new();

				for (int ty = 0; ty < Random.Range(minHeight, maxHeight + 1); ty++) {
					stateOverrides[trunkPos] = woodTile;
					trunkPos.y++;
				}

				Dictionary<int, List<int>> rowToXs = new();
				float angularStep = 1f;
				for (float angle = 0; angle < 360f; angle += angularStep) {
					float rad = angle * Mathf.Deg2Rad;
					int x = Mathf.RoundToInt(trunkPos.x + crownRadius * Mathf.Cos(rad));
					int y = Mathf.RoundToInt(trunkPos.y + crownRadius * Mathf.Sin(rad));
					if (!rowToXs.ContainsKey(y)) {
						rowToXs[y] = new List<int>();
					}
					rowToXs[y].Add(x);
				}
				foreach (var kvp in rowToXs) {
					int y = kvp.Key;
					List<int> xs = kvp.Value;
					for (int x = xs.Min(); x <= xs.Max(); x++) {
						BlockPos blockPos = new(ToWorldX(x, chunkX), y);
						ChunkBlockPos leafChunkPos = blockPos.ToChunkBlockPos(ChunkXAt(blockPos.x));
						stateOverrides[leafChunkPos] = leafTile;
					}
				}

				TerrainFeature feature = new(treeOrigin, type, stateOverrides);
				return feature;
			}

			default: return default(TerrainFeature);
		}
	}

	public bool FeatureAt(BlockPos blockPos, out TerrainFeature feature) {
		int chunkX = ChunkXAt(blockPos.AsVector());
		if (features.TryGetValue(chunkX, out var chunkFeatures)) {
			ChunkBlockPos chunkBlockPos = blockPos.ToChunkBlockPos(chunkX);
			feature = chunkFeatures.FirstOrDefault(f => f.origin == chunkBlockPos);
			if (!feature.PersistentExistence()) {
				feature = chunkFeatures.FirstOrDefault(f => f.stateOverrides.ContainsKey(chunkBlockPos));
			}
            return feature.PersistentExistence();
        }
		feature = default(TerrainFeature);
		return false;
    }

	public void SetBlockAndUpdate(BlockPos blockPos, [CanBeNull] BlockState blockState) {
		WorldChunk chunk = this.ChunkAt(blockPos);
		TileBase referencedTile = blockState?.block.TileReference ?? CommonTiles.air;
        chunk.SetBlock(blockPos.ToChunkBlockPos(chunk.xpos), blockState);
        tilemap.SetTile((Vector3Int)blockPos.AsVector(), referencedTile);
	}

	public void SetBlockAndUpdate(ChunkBlockPos chunkBlockPos, [CanBeNull] BlockState blockState) => SetBlockAndUpdate(chunkBlockPos.ToWorldBlockPos(), blockState);

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
		}
	}

	[CanBeNull]
	public BlockState BlockStateAt(BlockPos blockPos, bool logFlag = true) {
		WorldChunk chunk = ChunkAt(blockPos);
		if (chunk != null) {
			return chunk.BlockAt(blockPos.ToChunkBlockPos(chunk.xpos));
		}
		logFlag.If(() => Debug.LogError($"Cannot retrieve block state at {blockPos.ToString()} because its not generated"));
		return null;
	}

	[CanBeNull]
	public Block BlockAt(BlockPos blockPos) {
		BlockState blockState = BlockStateAt(blockPos, logFlag: false);
		if (blockState != null) {
			return blockState.block;
		}
		Debug.LogError($"Cannot retrieve block at {blockPos.ToString()} because its not generated");
		return Blocks.air;
    }

    public int ChunkXAt(Vector2 worldPos) => Mathf.FloorToInt(worldPos.x / CHUNK_LENGTH);

	public int ChunkXAt(float x) => Mathf.FloorToInt(x / CHUNK_LENGTH);

	[CanBeNull] public WorldChunk ChunkAt(BlockPos blockPos) => generatedChunks.GetValueOrDefault(this.ChunkXAt(blockPos.AsVector()), null);

	[CanBeNull] public WorldChunk ChunkAt(int xpos) => ChunkAt(new BlockPos(xpos, 0));

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

	public bool IsInWorldBounds(BlockPos blockPos) => IsInWorldBounds(blockPos.AsVector());
}