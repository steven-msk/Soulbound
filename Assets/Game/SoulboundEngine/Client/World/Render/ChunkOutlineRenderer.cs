using SoulboundEngine.Client.World.Chunk;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundEngine.Client.World.Render {
	using Level = Level.Level;

	[PROTOTYPICAL]
	public sealed class ChunkOutlineRenderer {
		private Dictionary<WorldChunk, LineRenderer> outlines = new();

		public void ShowOutline(WorldChunk chunk) {
			GameObject obj = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("chunkOutline")));
			LineRenderer renderer = obj.GetComponent<LineRenderer>();
			this.outlines[chunk] = renderer;

			int startX = chunk.chunkX * Level.CHUNK_LENGTH;
			int height = Level.WORLD_HEIGHT;
			int width = Level.CHUNK_LENGTH;
			Vector3[] points = new Vector3[5] {
				new Vector3(startX, Level.MIN_Y, 0),
				new Vector3(startX, Level.MIN_Y + height, 0),
				new Vector3(startX + width, Level.MIN_Y + height, 0),
				new Vector3(startX + width, Level.MIN_Y, 0),
				new Vector3(startX, Level.MIN_Y, 0)
			};
			renderer.positionCount = points.Length;
			renderer.SetPositions(points);
			renderer.startColor = renderer.endColor = Color.green;
		}

		public void HideOutline(WorldChunk chunk) {
			if (this.outlines.TryGetValue(chunk, out var renderer)) {
				GameObject.Destroy(renderer.gameObject);
				this.outlines.Remove(chunk);
			}
		}

		public void Clear() {
			foreach (var entry in this.outlines) {
				GameObject.Destroy(entry.Value.gameObject);
			}
			this.outlines.Clear();
		}
	}
}
