using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

#nullable enable

public struct WorldDump {
	[JsonConverter(typeof(Vector2JsonConverter))] public Vector2 lastPlayerPos;
	public WorldChunk[]? generatedChunks;
	public Dictionary<int, List<StructurePlacement>> structurePlacements;

	public WorldDump(WorldChunk[] generatedChunks, Vector2 lastPlayerPos, Dictionary<int, List<StructurePlacement>> structurePlacements) {
		this.generatedChunks = generatedChunks;
		this.lastPlayerPos = lastPlayerPos;
		this.structurePlacements = structurePlacements;
	}
}
