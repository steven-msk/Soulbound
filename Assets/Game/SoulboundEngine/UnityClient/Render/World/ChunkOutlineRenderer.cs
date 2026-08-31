namespace SoulboundEngine.UnityClient.Render.World {
	using SoulboundEngine.UnityClient.Debug;
	using SoulboundEngine.World;
	using SoulboundEngine.World.Chunk;
	using SoulboundEngine.World.Level;
	using System.Collections.Generic;
	using UnityEngine;

	public sealed class ChunkOutlineRenderer {
		private readonly HashSet<Chunk> chunks = new();
		private readonly DebugRenderer debugRenderer;

		public ChunkOutlineRenderer(DebugRenderer debugRenderer) {
			this.debugRenderer = debugRenderer;
		}

		public void ShowOutline(Chunk chunk) {
			this.chunks.Add(chunk);
		}

		public void Render() {
			foreach (Chunk chunk in this.chunks) {
				int startX = chunk.GetPos().x * Level.CHUNK_LENGTH;
				int endX = startX + Level.CHUNK_LENGTH;
				this.debugRenderer.AddLine(new Vector3(startX, chunk.GetBottomY()), new Vector3(startX, chunk.GetTopY()), Color.green);
				this.debugRenderer.AddLine(new Vector3(endX, chunk.GetBottomY()), new Vector3(endX, chunk.GetTopY()), Color.green);
			}
		}

		public void HideOutline(Chunk chunk) {
			this.chunks.Remove(chunk);
		}

		public void Clear() {
			this.chunks.Clear();
		}
	}
}
