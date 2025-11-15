using System;
using UnityEngine;

namespace SoulboundBackend.Client.World.EntitySystem.AI {
	public class MoveToPositionState : IMutableAIState<MoveToPositionState> {
		public MutableStateProperty<Vector2> target { get; }
		private Entity entity;
		private float speed;

		public bool isInterruptable { get; protected set; }
		public bool isFinished { get; protected set; }

		public MoveToPositionState(Entity entity, Vector2 target, float speed, bool isInterruptable = false) {
			this.target = new MutableStateProperty<Vector2>(target);
			this.entity = entity;
			this.isInterruptable = isInterruptable;
			this.speed = speed;
		}

		public void OnEnter() {
			UnityEngine.Debug.Log("starting move");
			isFinished = false;
		}

		public void OnExit() {
			UnityEngine.Debug.Log("end move");
		}

		public void Tick() {
		}

		public void OnUpdate(float deltaTime) {
			Vector2 dir = (target.value - entity.position);
			if (dir.magnitude < 0.1f) {
				isFinished = true;
				return;
			}
			dir.Normalize();
			entity.transform.position += (Vector3)dir * deltaTime * speed;
		}

		public void Mutate(Action<MoveToPositionState> fieldUpdater) {
			fieldUpdater.Invoke(this);
		}

		public override bool Equals(object obj) {
			return obj is MoveToPositionState other && other.target == target && other.entity == entity;
		}

		public override int GetHashCode() {
			int hash = 17;
			hash += target.GetHashCode();
			hash += entity.GetHashCode();
			return hash;
		}
	}
}
