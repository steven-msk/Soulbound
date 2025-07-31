using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugInfoDisplay : MonoBehaviour {
	private PlayerController player;
	private Level level;

	[SerializeField] private VectorCoordinateVisual worldPos;
	[SerializeField] private BlockCoordinateVisual blockPos;
	[SerializeField] private ChunkCoordinateVisual chunkPos;

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
    }

	public string Coordinates {
		get {
			Vector2 worldPos = player.position;
			BlockPos blockPos = level.ToBlockPos(worldPos);
			int chunkX = level.ChunkXAt(worldPos);
			ChunkBlockPos chunkPos = blockPos.ToChunkBlockPos(chunkX);
			return $"x:{worldPos.x}, y:{worldPos.y}\n" +
				   $"{blockPos.ToString()}\n" +
				   $"{chunkPos.ToString()}";
		}
	}

	public string Velocity {
		get {
			Vector2 velocity = player.Rigidbody.linearVelocity;
			return $"Current velocity XY: {velocity.ToString()}";
		}
	}
}