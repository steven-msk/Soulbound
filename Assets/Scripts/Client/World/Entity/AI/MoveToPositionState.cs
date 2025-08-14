using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MoveToPositionState : IMutableAIState<MoveToPositionState> {
	public MutableStateProperty<Vector2> target { get; }
	private Entity entity;

	public bool isInterruptable { get; protected set; }
	public bool isFinished { get; protected set; }

	public MoveToPositionState(Entity entity, Vector2 target, bool isInterruptable = false) {
		this.target = new MutableStateProperty<Vector2>(target);
		this.entity = entity;
		this.isInterruptable = isInterruptable;
	}

	public void OnEnter() {
		Debug.Log("starting move");
		isFinished = false;
	}

	public void OnExit() {
		Debug.Log("end move");
	}

	public void Tick() {
		Vector2 dir = (target.value - entity.position);
		if (dir.magnitude < 0.1f) {
			isFinished = true;
			return;
		}
		dir.Normalize();
		entity.transform.position += (Vector3)dir * GameManager.tickRate * 10f;
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

	public void Mutate(Action<MoveToPositionState> fieldUpdater) {
		fieldUpdater.Invoke(this);
	}
}
