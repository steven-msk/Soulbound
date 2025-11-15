using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.Chunk;
using SoulboundBackend.Core;
using UnityEngine;

namespace SoulboundBackend.Client.Debug {
	public class DebugInfoDisplay : MonoBehaviour {
		private PlayerController player;
		private Level level;

		[SerializeField] private VectorCoordinateVisual worldPos;
		[SerializeField] private BlockCoordinateVisual blockPos;
		[SerializeField] private ChunkCoordinateVisual chunkPos;
		[SerializeField] private VectorCoordinateVisual velocity;

		private void Update() {
			var levelManager = Soulbound.instance?.GetActiveLevelManager();
			this.level = levelManager.level;
			this.player = levelManager.player;
			if (level == null || player == null) {
				return;
			}

			// FIXME: unexpected null reference exceptions with debug visual components
			// hot-fixed with null pattern checks
			worldPos?.UpdateDisplayComponent(worldPos, player.position);
			blockPos?.UpdateDisplayComponent(blockPos, player.blockPos);
			chunkPos?.UpdateDisplayComponent(chunkPos, ChunkBlockPos.FromBlockPos(player.blockPos));
			velocity?.UpdateDisplayComponent(velocity, player.Rigidbody.linearVelocity);
		}
	}
}