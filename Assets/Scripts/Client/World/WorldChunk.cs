using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldChunk {
	private int x;
	public int Xpos => x;

	private TileBase[,] tiles = new TileBase[Level.chunkSize, Level.worldHeight];
	public TileBase[,] Tiles => tiles;

	public bool IsGenerated { get; set; }

	public WorldChunk(int x) => this.x = x;

	// PLANNED REFACTOR: chunk generation logic - required when introducing biomes
	public void Generate() {
		int startX = x * Level.chunkSize;
		for (int x = 0; x < Level.chunkSize; x++) {
			int worldX = startX + x;

			// magic numbers o no
			float heightRange = Mathf.PerlinNoise1D(worldX * 0.01f);
			int groundHeight = Mathf.FloorToInt(heightRange * 50f);

			for (int y = 0; y < Level.worldHeight; y++) {
				Tiles[x, y] = y < groundHeight ? CommonTiles.grass : CommonTiles.air;
			}
		}
		IsGenerated = true;
	}

	public void Render(Tilemap tilemap) {
		int xStart = x * Level.chunkSize;
		for (int x = 0; x < Level.chunkSize; x++) {
			for (int y = 0; y < Level.worldHeight; y++) {
				TileBase tile = Tiles[x, y];
				InvocationHelper.IfElse(tile != null,
					() => tilemap.SetTile(new Vector3Int(xStart + x, y, 0), tile),
					() => Debug.LogError($"Attempted to render ungenerated terrain! pos: ({x}, {y}) at chunk {this.x}"));
			}
		}
	}

	public void Unload(Tilemap tilemap) {
		int xStart = x * Level.chunkSize;
		for (int x = 0; x < Level.chunkSize; x++) {
			for (int y = 0; y < Level.worldHeight; y++) {
				tilemap.SetTile(new Vector3Int(xStart + x, y, 0), null);
			}
		}
	}

	public Vector2Int ToChunkBlock(Vector2 pos) {
		Level level = GameManager.instance.Level;
		int chunkX = level.ChunkXAt(pos);
		int chunkBlockX = Mathf.FloorToInt(pos.x - (chunkX * Level.chunkSize));
		return new Vector2Int(chunkBlockX, Mathf.FloorToInt(pos.y));
	}

	public TileBase TileAt(Vector2Int chunkPos) => Tiles[chunkPos.x, chunkPos.y];

	public TileBase TileAt(Vector2 worldPos) {
		return this.TileAt(this.ToChunkBlock(worldPos));
	}
}
