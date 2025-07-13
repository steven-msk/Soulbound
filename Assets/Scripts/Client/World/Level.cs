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
	public static readonly int worldHeight = 300;
	
	private Dictionary<int, WorldChunk> loadedChunks = new();
	private Dictionary<int, WorldChunk> generatedChunks = new();
	private ChunkOutlineRenderer chunkOutlineRenderer = new();

	private Grid grid;

	private PlayerController player;
	public PlayerController Player => player;

	private Tilemap tilemap;
	public Tilemap WorldTilemap => tilemap;

	public Level(PlayerController player, Tilemap tilemap) {
		this.player = player;
		this.tilemap = tilemap;
	}

	// FIXME: unoptimized terrain generation and rendering

	public void UpdateChunks(Vector2 playerPos) {
		int playerChunkX = ChunkXAt(playerPos);
		int viewDistance = 4;
		this.UnloadDistantChunks(playerChunkX, viewDistance);

		for (int dx = -viewDistance; dx <= viewDistance; dx++) {
			int chunkX = playerChunkX + dx;
			if (!loadedChunks.ContainsKey(chunkX)) {
				WorldChunk chunk = new(chunkX);
				if (!generatedChunks.ContainsKey(chunkX)) {
					chunk.Generate();
					generatedChunks[chunkX] = chunk;
				} else {
					chunk = generatedChunks[chunkX];
				}
				loadedChunks[chunkX] = chunk;
				chunk.Render(tilemap);
				chunkOutlineRenderer.ShowOutline(chunk);
			}
		}
	}

	public void UnloadDistantChunks(int playerChunkX, int viewDistance) {
		List<WorldChunk> toRemove = new();
		foreach (int chunkX in loadedChunks.Keys) {
			if (Mathf.Abs(chunkX - playerChunkX) > viewDistance) {
				toRemove.Add(loadedChunks[chunkX]);
			}
		}
		foreach (WorldChunk chunk in toRemove) {
			loadedChunks.Remove(chunk.Xpos);
			chunkOutlineRenderer.HideOutline(chunk);
			chunk.Unload(tilemap);
		}
	}

	[CanBeNull] public TileBase BlockAt(Vector2 worldPos) {
		int chunkX = ChunkXAt(worldPos);
		WorldChunk chunk = generatedChunks.GetValueOrDefault(chunkX, null);
		if (chunk != null) {
			return chunk.BlockAt(worldPos);
		}
		Debug.LogError($"Cannot retrieve block at {worldPos.ToString()} because its not generated");
		return null;
	}

	public int ChunkXAt(Vector2 worldPos) => Mathf.FloorToInt(worldPos.x / chunkSize);

	[CanBeNull] public WorldChunk ChunkAt(Vector2 worldPos) => generatedChunks.GetValueOrDefault(this.ChunkXAt(worldPos), null);

	// NOT TESTED
	public int HighestPoint(Vector2 worldPos) {
		Vector2 origin = new Vector2(worldPos.x, Level.worldHeight);
		RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, Level.worldHeight, LayerMask.GetMask("Ground"));
		if (hit.collider != null) {
			return grid.WorldToCell(hit.point).y;
		}
		return -1;
	}
}
