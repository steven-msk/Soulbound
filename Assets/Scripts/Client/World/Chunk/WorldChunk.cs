using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WorldChunk {
	public static readonly int minY = -Level.WORLD_HEIGHT / 2;
	public static readonly int maxY = Level.WORLD_HEIGHT / 2 - 1;
	public const float HEIGHT_SPREAD = 0.01f;
	public const float SURFACE_HEIGHT_RANGE = 50f;
	public const float UNDERGROUND_HEIGHT_RANGE = 20f;

	private ChunkHeightmapData heightmapData;
	public ChunkHeightmapData HeightmapData => heightmapData;

	private int x;
	public int xpos => x;

	private TileBase[,] tiles = new TileBase[Level.CHUNK_LENGTH, Level.WORLD_HEIGHT];


	public WorldChunk(int x) => this.x = x;

	// PLANNED REFACTOR: chunk generation logic - required when introducing biomes
	public ChunkHeightmapData GenerateHeightmap(INoiseGenerator1D heightGenerator) {
		int startX = x * Level.CHUNK_LENGTH;
		Dictionary<int, int> surfaceLevels = new();
		int highestStone = 0;
		Level level = GameManager.instance.Level;

		for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
			int worldX = startX + x;
			float heightNoise = heightGenerator.GenerateNoise1D(worldX);
			int groundHeight = Mathf.FloorToInt(heightNoise * SURFACE_HEIGHT_RANGE);
			int undergroundHeight = Mathf.FloorToInt(heightNoise * UNDERGROUND_HEIGHT_RANGE);
			surfaceLevels.Add(worldX, groundHeight + 1);

			highestStone = Mathf.Max(highestStone, undergroundHeight);
			for (int y = minY; y < maxY; y++) {
				int yIndex = WorldYToIndex(y);
				TileBase tile = default(TileBase);
				if (y > groundHeight) {
					tile = CommonTiles.air;
				} else if (y == groundHeight) {
					tile = CommonTiles.grass;
				} else if (y < groundHeight && y >= undergroundHeight) {
					tile = CommonTiles.dirt;
				} else {
					tile = CommonTiles.stone;
				}
				tiles[x, yIndex] = tile;
			}
		}

		ChunkHeightmapData generationData = new ChunkHeightmapData(surfaceLevels, highestStone);
		this.heightmapData = generationData;
		return generationData;
	}

	int WorldYToIndex(int worldY) => worldY - minY;

	int IndexToWorldY(int yIndex) => yIndex + minY;

	public void Render(Tilemap tilemap, ChunkOutlineRenderer outlineRenderer) {
		int xStart = x * Level.CHUNK_LENGTH;
		for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
			for (int y = minY; y < maxY; y++) {
				int yIndex = WorldYToIndex(y);
				TileBase tile = tiles[x, yIndex];
				InvocationHelper.IfElse(tile != null,
					() => tilemap.SetTile(new Vector3Int(xStart + x, y, 0), tile),
					() => Debug.LogError($"Attempted to render ungenerated terrain! pos: ({x}, {y}) at chunk {this.x}"));
			}
		}
		this.RefreshTiles(tilemap);
		outlineRenderer.ShowOutline(this);
	}

	public void RefreshTiles(Tilemap tilemap) {
		int xStart = x * Level.CHUNK_LENGTH;
		for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
			for (int y = minY; y < maxY; y++) {
				int yIndex = WorldYToIndex(y);
				tilemap.RefreshTile(new Vector3Int(xStart + x, y, 0));
			}
		}
	}

	public void Unload(Tilemap tilemap, ChunkOutlineRenderer outlineRenderer) {
		int xStart = x * Level.CHUNK_LENGTH;
		for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
			for (int y = minY; y < maxY; y++) {
				tilemap.SetTile(new Vector3Int(xStart + x, y, 0), null);
			}
		}
	}

	public void SetTile(ChunkBlockPos chunkPos, TileBase tile) => tiles[chunkPos.x, WorldYToIndex(chunkPos.y)] = tile;

	public TileBase TileAt(ChunkBlockPos chunkPos) => tiles[chunkPos.x, WorldYToIndex(chunkPos.y)];
}
