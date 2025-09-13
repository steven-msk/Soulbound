using UnityEngine;

public class DebugInfoDisplay : MonoBehaviour {
	private PlayerController player;
	private Level level;

	[SerializeField] private VectorCoordinateVisual worldPos;
	[SerializeField] private BlockCoordinateVisual blockPos;
	[SerializeField] private ChunkCoordinateVisual chunkPos;
	[SerializeField] private VectorCoordinateVisual velocity;

    private void Awake() {
		this.player = GameManager.instance.Player;
		this.level = GameManager.instance.Level;
	}

    // TODO: rework debug visuals - make it so its not painful to look at coordinates while moving

    private void Update() {

		// FIXME: unexpected null reference exceptions with debug visual components
		// hot-fixed with null pattern checks
		worldPos?.UpdateDisplayComponent(worldPos, player.position);
		blockPos?.UpdateDisplayComponent(blockPos, player.blockPos);
		chunkPos?.UpdateDisplayComponent(chunkPos, ChunkBlockPos.FromBlockPos(player.blockPos));
		velocity?.UpdateDisplayComponent(velocity, player.Rigidbody.linearVelocity);
    }
}