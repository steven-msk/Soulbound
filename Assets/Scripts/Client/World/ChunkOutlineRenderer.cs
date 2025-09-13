using System.Collections.Generic;
using UnityEngine;

public sealed class ChunkOutlineRenderer {
	private Dictionary<WorldChunk, LineRenderer> outlines = new();

	public void ShowOutline(WorldChunk chunk) {
		GameObject obj = GameObject.Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("chunkOutline"));
		LineRenderer renderer = obj.GetComponent<LineRenderer>();
		outlines[chunk] = renderer;

		int startX = chunk.xpos * Level.CHUNK_LENGTH;
		int height = Level.WORLD_HEIGHT;
		int width = Level.CHUNK_LENGTH;
		Vector3[] points = new Vector3[5] {
			new Vector3(startX, WorldChunk.minY, 0),
			new Vector3(startX, WorldChunk.minY + height, 0),
			new Vector3(startX + width, WorldChunk.minY + height, 0),
			new Vector3(startX + width, WorldChunk.minY, 0),
			new Vector3(startX, WorldChunk.minY, 0)
		};
		renderer.positionCount = points.Length;
		renderer.SetPositions(points);
		renderer.startColor = renderer.endColor = Color.green;
	}

	public void HideOutline(WorldChunk chunk) {
		if (outlines.TryGetValue(chunk, out var renderer)) {
			GameObject.Destroy(renderer.gameObject);
			outlines.Remove(chunk);
		}
	}

	public void Clear() {
		foreach (var entry in outlines) {
			GameObject.Destroy(entry.Value.gameObject);
		}
		outlines.Clear();
	}
}
