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

public class Level {
	public static readonly int chunkSize = 32;
	public static readonly Vector2Int worldSize = new(10_000, 300);
	
	private Dictionary<int, WorldChunk> loadedChunks = new();
	private Dictionary<int, WorldChunk> generatedChunks = new();

	private Grid grid;

	private PlayerController player;
	public PlayerController Player => player;

	private Tilemap tilemap;
	public Tilemap WorldTilemap => tilemap;

	public Level(PlayerController player, Tilemap tilemap) {
		this.player = player;
		this.tilemap = tilemap;
	}

	public void UpdateChunks(Vector2 playerPos) {
		int playerChunkX = ChunkAt(playerPos);
		int viewDistance = 4;

		for (int dx = -viewDistance; dx <= viewDistance; dx++) {
			int chunkX = playerChunkX + dx;
			if (!loadedChunks.ContainsKey(chunkX)) {
				WorldChunk chunk = new(chunkX);
				if (!generatedChunks.ContainsKey(chunkX)) {
					chunk.Generate();
					generatedChunks[chunkX] = chunk;
				}
				loadedChunks[chunkX] = chunk;
				chunk.Render(tilemap);
			}
		}
	}

	[CanBeNull] public TileBase BlockAt(Vector2 worldPos) {
		int chunkX = ChunkAt(worldPos);
		WorldChunk chunk = generatedChunks.GetValueOrDefault(chunkX, null);
		if (chunk != null) {
			return chunk.BlockAt(worldPos);
		}
		Debug.LogError($"Cannot retrieve block at {worldPos.ToString()} because its not generated");
		return null;
	}

	public int ChunkAt(Vector2 pos) => Mathf.FloorToInt(pos.x / chunkSize);

	// NOT TESTED
	public int HighestPoint(Vector2 worldPos) {
		Vector2 origin = new Vector2(worldPos.x, Level.worldSize.y);
		RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, Level.worldSize.y, LayerMask.GetMask("Ground"));
		if (hit.collider != null) {
			return grid.WorldToCell(hit.point).y;
		}
		return -1;
	}
}
