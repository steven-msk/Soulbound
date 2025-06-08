using System;
using Unity.VisualScripting;
using UnityEngine;

[Obsolete]
public class AttackController : MonoBehaviour {
	public bool done = true;

	private void OnEnable() {
		done = false;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log("Hit");
	}
}