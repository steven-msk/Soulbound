using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldChunk {
	private static readonly int minY = -Level.WORLD_HEIGHT / 2;
	private static readonly int maxY = Level.WORLD_HEIGHT / 2 - 1;
	public const float HEIGHT_SPREAD = 0.01f;
	public const float SURFACE_HEIGHT_RANGE = 50f;
	public const float UNDERGROUND_HEIGHT_RANGE = 20f;

	private int x;
	public int Xpos => x;

	private TileBase[,] tiles = new TileBase[Level.CHUNK_LENGTH, Level.WORLD_HEIGHT];

	public bool IsGenerated { get; set; }

	public WorldChunk(int x) => this.x = x;

	// PLANNED REFACTOR: chunk generation logic - required when introducing biomes
	public Dictionary<int, int> Generate(INoiseGenerator1D heightGenerator) {
		int startX = x * Level.CHUNK_LENGTH;
		Dictionary<int, int> surfaceLevels = new();
		for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
			int worldX = startX + x;
			float heightNoise = heightGenerator.GenerateNoise1D(worldX);
			int groundHeight = Mathf.FloorToInt(heightNoise * SURFACE_HEIGHT_RANGE);
			int undergroundHeight = Mathf.FloorToInt(heightNoise * UNDERGROUND_HEIGHT_RANGE);
			surfaceLevels.Add(worldX, groundHeight + 1);

			for (int y = minY; y < maxY; y++) {
				int yIndex = WorldYToIndex(y);
				TileBase tile = default(TileBase);
				if (y >= groundHeight) {
					tile = CommonTiles.air;
				} else if (y < groundHeight && y >= undergroundHeight) {
					tile = CommonTiles.grass;
				} else {
					tile = CommonTiles.stone;
				}
				tiles[x, yIndex] = tile;
			}
		}
		IsGenerated = true;
		return surfaceLevels;
	}

	int WorldYToIndex(int worldY) => worldY - minY;

	int IndexToWorldY(int yIndex) => yIndex + minY;

	public void Render(Tilemap tilemap) {
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
	}

	public void Unload(Tilemap tilemap) {
		int xStart = x * Level.CHUNK_LENGTH;
		for (int x = 0; x < Level.CHUNK_LENGTH; x++) {
			for (int y = minY; y < maxY; y++) {
				tilemap.SetTile(new Vector3Int(xStart + x, y, 0), null);
			}
		}
	}

	public Vector2Int ToChunkBlock(Vector2 pos) {
		Level level = GameManager.instance.Level;
		int chunkX = level.ChunkXAt(pos);
		int chunkBlockX = Mathf.FloorToInt(pos.x - (chunkX * Level.CHUNK_LENGTH));
		return new Vector2Int(chunkBlockX, Mathf.FloorToInt(pos.y));
	}

	public void SetTile(Vector2Int worldPos, TileBase tile) => tiles[worldPos.x, WorldYToIndex(worldPos.y)] = tile;

	public TileBase TileAt(Vector2Int chunkPos) => tiles[chunkPos.x, WorldYToIndex(chunkPos.y)];

	public TileBase TileAt(Vector2 worldPos) => this.TileAt(this.ToChunkBlock(worldPos));
}
