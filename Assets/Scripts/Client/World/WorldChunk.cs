using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldChunk {
	private static int minY = -Level.worldHeight / 2;
	private static int maxY = Level.worldHeight / 2 - 1;
	private static float surfaceHeightSpread = 0.01f;
	private static float undergroundHeightSpread = 0.01f;
	private static float surfaceHeightRange = 50f;
	private static float undergroundHeightRange = 20f;

	private int x;
	public int Xpos => x;

	private TileBase[,] tiles = new TileBase[Level.chunkLength, Level.worldHeight];

	public bool IsGenerated { get; set; }

	public WorldChunk(int x) => this.x = x;

	// PLANNED REFACTOR: chunk generation logic - required when introducing biomes
	public Dictionary<int, int> Generate() {
		int startX = x * Level.chunkLength;
		Dictionary<int, int> surfaceLevels = new();
		for (int x = 0; x < Level.chunkLength; x++) {
			int worldX = startX + x;
			float surfaceHeightNoise = Mathf.PerlinNoise1D(worldX * surfaceHeightSpread);
			int groundHeight = Mathf.FloorToInt(surfaceHeightNoise * surfaceHeightRange);
			float undergroundHeightNoise = Mathf.PerlinNoise1D(worldX * undergroundHeightSpread);
			int undergroundHeight = Mathf.FloorToInt(undergroundHeightNoise * undergroundHeightRange);
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
		int xStart = x * Level.chunkLength;
		for (int x = 0; x < Level.chunkLength; x++) {
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
		int xStart = x * Level.chunkLength;
		for (int x = 0; x < Level.chunkLength; x++) {
			for (int y = minY; y < maxY; y++) {
				tilemap.SetTile(new Vector3Int(xStart + x, y, 0), null);
			}
		}
	}

	public Vector2Int ToChunkBlock(Vector2 pos) {
		Level level = GameManager.instance.Level;
		int chunkX = level.ChunkXAt(pos);
		int chunkBlockX = Mathf.FloorToInt(pos.x - (chunkX * Level.chunkLength));
		return new Vector2Int(chunkBlockX, Mathf.FloorToInt(pos.y));
	}

	public void SetTile(Vector2Int worldPos, TileBase tile) => tiles[worldPos.x, WorldYToIndex(worldPos.y)] = tile;

	public TileBase TileAt(Vector2Int chunkPos) => tiles[chunkPos.x, WorldYToIndex(chunkPos.y)];

	public TileBase TileAt(Vector2 worldPos) => this.TileAt(this.ToChunkBlock(worldPos));
}
