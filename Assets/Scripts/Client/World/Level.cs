using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Level {
	public const int CHUNK_LENGTH = 32;
	public const int WORLD_HEIGHT = 300;
	public const int SURFACE_TO_UNDERGROUND_DELIMITER = WORLD_HEIGHT / 2;
	
	private readonly int seed;
	private readonly PerlinNoiseGenerator1D heightGenerator;
	private Dictionary<int, WorldChunk> loadedChunks = new();
	private Dictionary<int, WorldChunk> generatedChunks = new();
	private ChunkOutlineRenderer chunkOutlineRenderer = new();
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
			WorldChunk chunk = new(chunkX);
			chunk.Generate(this.heightGenerator);
			generatedChunks[chunkX] = chunk;
			loadedChunks[chunkX] = chunk;
			chunk.Render(tilemap, chunkOutlineRenderer);
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
					chunk.Generate(this.heightGenerator);
					generatedChunks[chunkX] = chunk;
				} else {
					chunk = generatedChunks[chunkX];
				}
				loadedChunks[chunkX] = chunk;
				chunk.Render(tilemap, chunkOutlineRenderer);
			}
		}
	}

	public void SetBlockAndUpdate(BlockPos tilePos, [CanBeNull] Block block) {
		WorldChunk chunk = this.ChunkAt(tilePos);
		ChunkBlockPos chunkPos = tilePos.ToChunkBlockPos(chunk.xpos);
		TileBase referenceTile = block?.TileReference ?? CommonTiles.air;
		chunk.SetTile(chunkPos, referenceTile);
		tilemap.SetTile((Vector3Int)tilePos.AsVector(), referenceTile);
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
			chunkOutlineRenderer.HideOutline(chunk);
			chunk.Unload(tilemap, chunkOutlineRenderer);
		}
	}


	[CanBeNull] public TileBase TileAt(BlockPos blockPos) {
		WorldChunk chunk = ChunkAt(blockPos);
		if (chunk != null) {
			return chunk.TileAt(blockPos.ToChunkBlockPos(chunk.xpos));
		}
		Debug.LogError($"Cannot retrieve block at {blockPos.ToString()} because its not generated");
		return null;
	}

	public int ChunkXAt(Vector2 worldPos) => Mathf.FloorToInt(worldPos.x / CHUNK_LENGTH);

	[CanBeNull] public WorldChunk ChunkAt(BlockPos blockPos) => generatedChunks.GetValueOrDefault(this.ChunkXAt(blockPos.AsVector()), null);

	[CanBeNull] public WorldChunk ChunkAt(int xpos) => ChunkAt(new BlockPos(xpos, 0));

	public BlockPos ToBlockPos(Vector2 worldPos) { 
		Vector2Int intPos = (Vector2Int)grid.WorldToCell(worldPos); 
		return new BlockPos(intPos.x, intPos.y);
	}

	public int GetSurfaceY(BlockPos blockPos) => GetSurfaceY(blockPos.x);

	public int GetSurfaceY(int xpos) => this.ChunkAt(xpos)?.GenerationData.surfaceLevels[xpos] ?? 0;
}
