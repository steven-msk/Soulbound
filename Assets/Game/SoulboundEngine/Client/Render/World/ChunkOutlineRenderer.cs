namespace SoulboundEngine.Client.Render.World {
	using SoulboundEngine.Client.Assets;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Level;
	using System.Collections.Generic;
	using UnityEngine;

	public sealed class ChunkOutlineRenderer {
		private readonly Dictionary<Chunk, LineRenderer> outlines = new();

		public void ShowOutline(Chunk chunk) {
			GameObject obj = GameObject.Instantiate(AssetManager.Resolve<GameObject>(new AssetKey("chunkOutline")));
			LineRenderer renderer = obj.GetComponent<LineRenderer>();
			this.outlines[chunk] = renderer;

			int startX = chunk.GetPos().x * Level.CHUNK_LENGTH;
			int height = Level.WORLD_HEIGHT;
			int width = Level.CHUNK_LENGTH;
			Vector3[] points = new Vector3[5] {
				new(startX, Level.MIN_Y, 0),
				new(startX, Level.MIN_Y + height, 0),
				new(startX + width, Level.MIN_Y + height, 0),
				new(startX + width, Level.MIN_Y, 0),
				new(startX, Level.MIN_Y, 0)
			};
			renderer.positionCount = points.Length;
			renderer.SetPositions(points);
			renderer.startColor = renderer.endColor = Color.green;
		}

		public void HideOutline(Chunk chunk) {
			if (this.outlines.TryGetValue(chunk, out LineRenderer renderer)) {
				GameObject.Destroy(renderer.gameObject);
				this.outlines.Remove(chunk);
			}
		}

		public void Clear() {
			foreach ((Chunk _, LineRenderer gameObject) in this.outlines) {
				GameObject.Destroy(gameObject);
			}
			this.outlines.Clear();
		}
	}
}
