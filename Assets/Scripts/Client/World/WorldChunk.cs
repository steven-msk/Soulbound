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
		TileBase grass = Registry.Get<RuleTile>("grass");
		TileBase air = Registry.Get<Tile>("air");

		int startX = x * Level.chunkSize;
		for (int x = 0; x < Level.chunkSize; x++) {
			int worldX = startX + x;

			// magic numbers o no
			float heightRange = Mathf.PerlinNoise1D(worldX * 0.01f);
			int groundHeight = Mathf.FloorToInt(heightRange * 50f + 140f);

			for (int y = 0; y < Level.worldHeight; y++) {
				Tiles[x, y] = y < groundHeight ? grass : air;
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
					() => tilemap.SetTile(new Vector3Int(xStart + x, y - Level.worldHeight / 2, 0), tile),
					() => Debug.LogError($"Attempted to render ungenerated terrain! pos: ({x}, {y}) at chunk {this.x}"));
			}
		}
	}

	public void Unload(Tilemap tilemap) {
		int xStart = x * Level.chunkSize;
		for (int x = 0; x < Level.chunkSize; x++) {
			for (int y = 0; y < Level.worldHeight; y++) {
				tilemap.SetTile(new Vector3Int(xStart + x, y - Level.worldHeight / 2, 0), null);
			}
		}
	}

	public Vector2 ToChunkPos(Vector2 worldPos) => new Vector2(worldPos.x / Level.chunkSize, worldPos.y);

	public Vector2 ToWorldPos(Vector2 chunkPos) => new Vector2(chunkPos.x * Level.chunkSize, chunkPos.y);

	public TileBase BlockAt(Vector2Int chunkPos) => Tiles[chunkPos.x, chunkPos.y];

	public TileBase BlockAt(Vector2 worldPos) => BlockAt(this.ToChunkPos(worldPos));
}
