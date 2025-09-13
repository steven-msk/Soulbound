using UnityEngine;

public class IdleState : IAIState {
	public bool isInterruptable => true;
	public bool isFinished => false;

	public void OnEnter() {
		Debug.Log("entered idle");
	}

	public void OnExit() {
		Debug.Log("exiting idle");
	}

	public void Tick() {
		Debug.Log("idling");
	}

	public void OnUpdate(float deltaTime) {
	}

	public override bool Equals(object obj) {
		return obj is IdleState;
	}

	public override int GetHashCode() {
		int hash = 17;
		hash += (isInterruptable ? 1 : 0);
		hash += (isFinished ? 1 : 0);
		return hash;
	}

}
