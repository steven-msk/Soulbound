using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct TerrainFeature {
	public ChunkBlockPos origin;
	public TerrainFeatureType type;
	public Dictionary<ChunkBlockPos, TileBase> tileOverrides;

	public TerrainFeature(ChunkBlockPos origin, TerrainFeatureType type, Dictionary<ChunkBlockPos, TileBase> tileOverrides) {
		this.origin = origin;
		this.type = type;
		this.tileOverrides = tileOverrides;
	}

	public void OverrideTiles() {
		Level level = GameManager.instance.Level;

		foreach (var tileOverride in tileOverrides) {
			ChunkBlockPos chunkBlockPos = tileOverride.Key;
			TileBase tile = tileOverride.Value;
			WorldChunk chunk = level.ChunkAt(chunkBlockPos.ToWorldBlockPos());
			//Debug.Log(chunkBlockPos.ToWorldBlockPos() + $", chunkPos [{chunkBlockPos}]"+", chunk "+ chunk?.xpos);
			InvocationHelper.IfElse(chunk == null, 
				() => level.PendUpdate(chunkBlockPos.chunkX, chunkBlockPos, tile), 
				() => chunk.SetTile(chunkBlockPos, tile));
		}
	}
}