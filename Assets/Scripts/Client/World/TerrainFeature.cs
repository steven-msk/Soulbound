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
}