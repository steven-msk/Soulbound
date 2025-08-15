using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public sealed class AIEntity_test : Entity, ITickable {
	private AIController aiController;

	public override void Spawn(EntitySpawnData spawnData) {
		base.ValidateSpawnData<EntitySpawnData>(spawnData, (spawnData) => {
			transform.position = spawnData.position;
		});
		IdleState idle = new();
		PlayerController player = GameManager.instance.Player;
		MoveToPositionState move = new(this, player.position, speed: 5f, isInterruptable: true);
		aiController = new(this, () => {
			if (Vector2.Distance(position, player.position) < 10f) {
				move.Mutate((moveState) => moveState.target.MutateValue(player.position));
				return move;
			}
			return idle;
		}, idle);
	}

	public override void EntityUpdate(float deltaTime) {
		aiController.UpdateCurrentState(deltaTime);
	}

	public void Tick() {
		aiController.Tick();
	}

	public override Bounds GetBounds() => this.GetColliderBounds();

	public override void OnChunkLoaded() {
		gameObject.SetActive(true);
	}

	public override void OnChunkUnloaded() {
		gameObject.SetActive(false);
	}
}
