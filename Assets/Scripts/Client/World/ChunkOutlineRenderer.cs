using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class ChunkOutlineRenderer {
	private Dictionary<WorldChunk, LineRenderer> outlines = new();

	public void ShowOutline(WorldChunk chunk) {
		GameObject obj = GameObject.Instantiate(Registry.Get<GameObject>("chunkOutline"));
		LineRenderer renderer = obj.GetComponent<LineRenderer>();
		outlines[chunk] = renderer;

		int startX = chunk.Xpos * Level.chunkSize;
		int offsetY = -140;
		int height = Level.worldHeight;
		int width = Level.chunkSize;
		Vector3[] points = new Vector3[5] {
			new Vector3(startX, offsetY, 0),
			new Vector3(startX, height + offsetY, 0),
			new Vector3(startX + width, height + offsetY, 0),
			new Vector3(startX + width, offsetY, 0),
			new Vector3(startX, offsetY, 0)
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
