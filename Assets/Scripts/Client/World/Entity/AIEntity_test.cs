using SoulboundBackend.Client.World.EntitySystem.AI;
using SoulboundBackend.Core;
using System;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem {
	public sealed class AIEntity_test : Entity, ITickable {
		public override Type entityScriptType => typeof(AIEntity_test);
		public override string prefabDefinitionID => "ai test";

		public override float facing { 
			get => this.GetFacingFromXScaleSign();
			set => this.SetFacingUsingXScaleSign(value);
		}

		private AIController aiController;

		public override void Spawn(EntitySpawnData spawnData) {
			base.Spawn(spawnData);
			IdleState idle = new();
			PlayerController player = Soulbound.instance.GetPlayerInstance();
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

		public override SerializedEntityPropertyList GetSerializedProperties() => SerializedEntityPropertyList.Empty();

		public override void ApplySerializedProperties(SerializedEntityPropertyList properties) {
		}
	}
}
